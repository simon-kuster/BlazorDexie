using Nosthy.Blazor.DexieWrapper.JsInterop;

namespace Nosthy.Blazor.DexieWrapper.Database
{
    public class Collection<T, TKey>
    {
        protected Db Db = null!;
        protected CommandExecuterJsInterop CommandExecuterJsInterop = null!;
        protected string StoreName = null!;
        protected virtual List<Command> CurrentCommands { get; } = new List<Command>();

        protected Collection()
        {
        }

        public Collection(Db db, string storeName, CommandExecuterJsInterop commandExecuterJsInterop)
        {
            Init(db, storeName, commandExecuterJsInterop);
        }

        public void Init(Db db, string storeName, CommandExecuterJsInterop commandExecuterJsInterop)
        {
            Db = db;
            StoreName = storeName;
            CommandExecuterJsInterop = commandExecuterJsInterop;
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
            return await Count(Db.DefaultDatabaseName, cancellationToken);
        }

        public async Task<int> Count(string databaseName, CancellationToken cancellationToken = default)
        {
            return await Execute<int>("count", databaseName, cancellationToken);
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

        public Collection<T, TKey> Reverse()
        {
            var collection = CreateNewColletion();
            collection.AddCommand("reverse");
            return collection;
        }

        public async Task<T[]> ToArray(CancellationToken cancellationToken = default)
        {
            return await ToArray(Db.DefaultDatabaseName, cancellationToken);  
        }

        public async Task<T[]> ToArray(string databaseName, CancellationToken cancellationToken = default)
        {
            return await Execute<T[]>("toArray", databaseName ?? Db.DefaultDatabaseName, cancellationToken);
        }

        public async Task<List<T>> ToList(CancellationToken cancellationToken = default)
        {
            return await ToList(Db.DefaultDatabaseName, cancellationToken);
        }

        public async Task<List<T>> ToList(string databaseName, CancellationToken cancellationToken = default)
        {
            return await Execute<List<T>>("toArray", databaseName ?? Db.DefaultDatabaseName, cancellationToken);
        }

        protected virtual Collection<T, TKey> CreateNewColletion()
        {
            return this;
        }

        protected async Task<TRet> Execute<TRet>(string command, string databaseName, CancellationToken cancellationToken, params object?[] parameters)
        {
            var commands = CurrentCommands.ToList();
            commands.Add(new Command(command, parameters));

            await Db.Init(databaseName, cancellationToken);

            if (typeof(TRet) == typeof(Guid))
            {
                string retString;

                if (Db.DbJsReference != null)
                {
                    retString = await CommandExecuterJsInterop.Execute<string>(Db.DbJsReference, StoreName, commands, cancellationToken);
                }
                else
                {
                    retString = await CommandExecuterJsInterop.InitDbAndExecute<string>(databaseName, Db.Versions, StoreName, commands, cancellationToken);
                }

                return (TRet)(object)Guid.Parse(retString);
            }
            else
            {
                if (Db.DbJsReference != null)
                {
                    return await CommandExecuterJsInterop.Execute<TRet>(Db.DbJsReference, StoreName, commands, cancellationToken);
                }

                return await CommandExecuterJsInterop.InitDbAndExecute<TRet>(databaseName, Db.Versions, StoreName, commands, cancellationToken);
            }
        }

        protected async Task ExecuteNonQuery(string command, string databaseName, CancellationToken cancellationToken, params object?[] parameters)
        {
            var commands = CurrentCommands.ToList();
            commands.Add(new Command(command, parameters));

            await Db.Init(databaseName, cancellationToken);

            if (Db.DbJsReference != null)
            {
                await CommandExecuterJsInterop.ExecuteNonQuery(Db.DbJsReference, StoreName, commands, cancellationToken);
            }
            else
            {
                await CommandExecuterJsInterop.InitDbAndExecuteNonQuery(databaseName, Db.Versions, StoreName, commands, cancellationToken);
            }
        }
    }
}
