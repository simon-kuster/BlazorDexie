using Nosthy.Blazor.DexieWrapper.Blob;
using Nosthy.Blazor.DexieWrapper.DexieJsInterop;

namespace Nosthy.Blazor.DexieWrapper.Database
{
    public class Collection<T, TKey>
    {
        protected BlobDataConvertFactory<T> BlobDataConvertFactory = new();
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

        public void AddCommand(string command, params object?[] dexieParameters)
        {
            CurrentCommands.Add(new Command(command, dexieParameters, new CommandParameters()));
        }

        public Collection<T, TKey> And(string filterFunction, IEnumerable<object>? dexieParameters = null)
        {
            return Filter(filterFunction, dexieParameters);
        }

        public async Task<int> Count(CancellationToken cancellationToken = default)
        {
            return await Count(Db.DefaultDatabaseName, cancellationToken);
        }

        public async Task<int> Count(string databaseName, CancellationToken cancellationToken = default)
        {
            return await Execute<int>("count", new object?[] { }, databaseName, new CommandParameters(), cancellationToken);
        }

        public Collection<T, TKey> Filter(string filterFunction, IEnumerable<object>? dexieParameters = null)
        {
            var collection = CreateNewColletion();
            collection.AddCommand("filter", filterFunction, dexieParameters);
            return collection;
        }

        public Collection<T, TKey> FilterModule(string modulePath, IEnumerable<object>? dexieParameters = null)
        {
            var collection = CreateNewColletion();
            collection.AddCommand("filterModule", modulePath, dexieParameters);
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

        public async Task<T[]> ToArray(BlobDataFormat blobDataFormat = BlobDataFormat.ByteArray, CancellationToken cancellationToken = default)
        {
            return await ToArray(Db.DefaultDatabaseName, blobDataFormat, cancellationToken);  
        }

        public async Task<T[]> ToArray(string databaseName, BlobDataFormat blobDataFormat = BlobDataFormat.ByteArray, CancellationToken cancellationToken = default)
        {
            return await Execute<T[]>("toArray",
                new object?[] { },
                databaseName,
                new CommandParameters { BlobDataConvert = BlobDataConvertFactory.CreateForRead(true, blobDataFormat) },
                cancellationToken);
        }

        public async Task<List<T>> ToList(CancellationToken cancellationToken = default)
        {
            return await ToList(Db.DefaultDatabaseName, cancellationToken);
        }

        public async Task<List<T>> ToList(string databaseName, CancellationToken cancellationToken = default)
        {
            return await Execute<List<T>>(
                "toArray", new object?[] { }, 
                databaseName, new CommandParameters(), 
                cancellationToken);
        }

        protected virtual Collection<T, TKey> CreateNewColletion()
        {
            return this;
        }

        protected async Task<TRet> Execute<TRet>(string command, object?[] dexieParameters, string databaseName, CommandParameters commandParameters, CancellationToken cancellationToken)
        {
            var commands = CurrentCommands.ToList();
            commands.Add(new Command(command, dexieParameters, commandParameters));

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

        protected async Task ExecuteNonQuery(string command, object?[] dexieParameters, string databaseName, CommandParameters commandParameters, CancellationToken cancellationToken)
        {
            var commands = CurrentCommands.ToList();
            commands.Add(new Command(command, dexieParameters, new CommandParameters()));

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
