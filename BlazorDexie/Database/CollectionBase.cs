using BlazorDexie.JsInterop;

namespace BlazorDexie.Database
{
    public abstract class CollectionBase<T, TKey>
    {
        protected Db Db = null!;
        protected CollectionCommandExecuterJsInterop CommandExecuterJsInterop = null!;
        protected string StoreName = null!;
        protected virtual List<Command> CurrentCommands { get; } = new List<Command>();

        public void Init(Db db, string storeName, CollectionCommandExecuterJsInterop commandExecuterJsInterop)
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

        protected abstract Collection<T, TKey> CreateNewColletion();

        protected async Task<TRet> Execute<TRet>(string command, CancellationToken cancellationToken, params object?[] parameters)
        {
            var commands = CurrentCommands.ToList();
            commands.Add(new Command(command, parameters));

            await Db.Init(cancellationToken);

            if (typeof(TRet) == typeof(Guid))
            {
                string retString;

                if (Db.DbJsReference != null)
                {
                    retString = await CommandExecuterJsInterop.Execute<string>(Db.DbJsReference, StoreName, commands, cancellationToken);
                }
                else
                {
                    retString = await CommandExecuterJsInterop.InitDbAndExecute<string>(Db.DatabaseName, Db.Versions, StoreName, commands, cancellationToken);
                }

                return (TRet)(object)Guid.Parse(retString);
            }
            else
            {
                if (Db.DbJsReference != null)
                {
                    return await CommandExecuterJsInterop.Execute<TRet>(Db.DbJsReference, StoreName, commands, cancellationToken);
                }

                return await CommandExecuterJsInterop.InitDbAndExecute<TRet>(Db.DatabaseName, Db.Versions, StoreName, commands, cancellationToken);
            }
        }

        protected async Task ExecuteNonQuery(string command, CancellationToken cancellationToken, params object?[] parameters)
        {
            var commands = CurrentCommands.ToList();
            commands.Add(new Command(command, parameters));

            await Db.Init(cancellationToken);

            if (Db.DbJsReference != null)
            {
                await CommandExecuterJsInterop.ExecuteNonQuery(Db.DbJsReference, StoreName, commands, cancellationToken);
            }
            else
            {
                await CommandExecuterJsInterop.InitDbAndExecuteNonQuery(Db.DatabaseName, Db.Versions, StoreName, commands, cancellationToken);
            }
        }
    }
}
