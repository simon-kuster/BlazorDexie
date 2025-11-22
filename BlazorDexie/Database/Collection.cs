using BlazorDexie.Database.Transaction;
using BlazorDexie.JsInterop;
using BlazorDexie.Logging;
using Microsoft.Extensions.Logging;

namespace BlazorDexie.Database
{
    public class Collection<T, TKey>
    {
        protected IDb Db = null!;
        protected CollectionCommandExecuterJsInterop CommandExecuterJsInterop = null!;
        protected TransactionBodyWrapper? TransactionBodyWrapper;
        protected string StoreName = null!;
        protected virtual List<Command> CurrentCommands { get; } = new List<Command>();
        protected ILogger _logger = null!;

        protected Collection()
        {
        }

        public Collection(IDb db, string storeName, CollectionCommandExecuterJsInterop commandExecuterJsInterop, ILogger logger)
        {
            Init(db, storeName, commandExecuterJsInterop, logger);
        }

        public void Init(IDb db, string storeName, CollectionCommandExecuterJsInterop commandExecuterJsInterop, ILogger logger)
        {
            Db = db;
            StoreName = storeName;
            CommandExecuterJsInterop = commandExecuterJsInterop;
            _logger = logger;
        }

        public void SetTransactionWrapper(TransactionBodyWrapper transactionBodyWrapper)
        {
            TransactionBodyWrapper = transactionBodyWrapper;
        }

        public void ClearTransactionWrapper()
        {
            TransactionBodyWrapper = null;
        }

        public void AddCommand(string command, params object?[] parameters)
        {
            CurrentCommands.Add(new Command(command, parameters));
        }

        public Collection<T, TKey> And(string filterFunction, IEnumerable<object>? parameters = null)
        {
            return Filter(filterFunction, parameters);
        }

        public async Task<int> Count(CancellationToken cancellationToken = default)
        {
            return await Execute<int>("count", cancellationToken);
        }

        public Collection<T, TKey> Filter(string filterFunction, IEnumerable<object>? parameters = null)
        {
            var collection = CreateNewColletion();
            collection.AddCommand("filter", filterFunction, parameters);
            return collection;
        }

        public Collection<T, TKey> FilterModule(string modulePath, IEnumerable<object>? parameters = null)
        {
            var collection = CreateNewColletion();
            collection.AddCommand("filterModule", modulePath, parameters);
            return collection;
        }

        public async Task<TIndex[]> Keys<TIndex>(CancellationToken cancellationToken = default)
        {
            return await Execute<TIndex[]>("keys", cancellationToken);
        }

        public Collection<T, TKey> Limit(int count)
        {
            var collection = CreateNewColletion();
            collection.AddCommand("limit", count);
            return collection;
        }

        public Collection<T, TKey> Offset(int count)
        {
            var collection = CreateNewColletion();
            collection.AddCommand("offset", count);
            return collection;
        }

        public virtual async Task<TKey[]> PrimaryKeys(CancellationToken cancellationToken = default)
        {
            return await Execute<TKey[]>("primaryKeys", cancellationToken);
        }

        public Collection<T, TKey> Reverse()
        {
            var collection = CreateNewColletion();
            collection.AddCommand("reverse");
            return collection;
        }

        public async Task<T[]> ToArray(CancellationToken cancellationToken = default)
        {
            return await Execute<T[]>("toArray", cancellationToken);
        }

        public async Task<List<T>> ToList(CancellationToken cancellationToken = default)
        {
            return await Execute<List<T>>("toArray", cancellationToken);
        }

        public virtual async Task<T[]> SortBy(string keyPath, CancellationToken cancellationToken = default)
        {
            return await Execute<T[]>("sortBy", cancellationToken, keyPath);
        }

        public virtual async Task<List<T>> SortByToList(string keyPath, CancellationToken cancellationToken = default)
        {
            return await Execute<List<T>>("sortBy", cancellationToken, keyPath);
        }

        protected virtual Collection<T, TKey> CreateNewColletion()
        {
            return this;
        }

        protected async Task<TRet> Execute<TRet>(string command, CancellationToken cancellationToken, params object?[] parameters)
        {
            var commands = CurrentCommands.ToList();
            commands.Add(new Command(command, parameters));

            if (TransactionBodyWrapper == null)
            {
                await Db.Init(cancellationToken);
            }

            var commandLogger = new StoreCommandLogger(_logger, LogLevel.Information);
            commandLogger.Start();

            if (typeof(TRet) == typeof(Guid))
            {
                string returnString = await ExecuteInternal<string>(commands, cancellationToken);
                commandLogger.Log(StoreName, commands);
                return (TRet)(object)Guid.Parse(returnString);
            }
            else
            {
                TRet returnValue = await ExecuteInternal<TRet>(commands, cancellationToken);
                commandLogger.Log(StoreName, commands);
                return returnValue;
            }
        }

        protected async Task ExecuteNonQuery(string command, CancellationToken cancellationToken, params object?[] parameters)
        {
            var commands = CurrentCommands.ToList();
            commands.Add(new Command(command, parameters));

            await Db.Init(cancellationToken);

            var commandLogger = new StoreCommandLogger(_logger, LogLevel.Information);
            commandLogger.Start();

            if (TransactionBodyWrapper != null)
            {
                await TransactionBodyWrapper.ExecuteNonQuery(StoreName, commands, cancellationToken);
            }
            else if (Db.DbJsReference != null)
            {
                await CommandExecuterJsInterop.ExecuteNonQuery(Db.DbJsReference, StoreName, commands, cancellationToken);
            }
            else
            {
                await CommandExecuterJsInterop.InitDbAndExecuteNonQuery(Db.DatabaseName, Db.Versions, StoreName, commands, cancellationToken);
            }

            commandLogger.Log(StoreName, commands);
        }

        private Task<TRet> ExecuteInternal<TRet>(List<Command> commands, CancellationToken cancellationToken)
        {
            if (TransactionBodyWrapper != null)
            {
                return TransactionBodyWrapper.Execute<TRet>(StoreName, commands, cancellationToken);
            }
            else if (Db.DbJsReference != null)
            {
                return CommandExecuterJsInterop.Execute<TRet>(Db.DbJsReference, StoreName, commands, cancellationToken);
            }
            else
            {
                return CommandExecuterJsInterop.InitDbAndExecute<TRet>(Db.DatabaseName, Db.Versions, StoreName, commands, cancellationToken);
            }
        }
    }
}
