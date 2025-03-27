using BlazorDexie.Database;
using BlazorDexie.Demo.Persons;
using BlazorDexie.Options;

namespace BlazorDexie.Demo.Database
{
    public class MyDb : Db<MyDb>
    {
        public Store<Person, Guid> Persons { get; set; } = new(nameof(Person.Id));

        public Store<byte[], Guid> BlobData { get; set; } = new(string.Empty);
        public Store<byte[], Guid> BlobData2 { get; set; } = new(string.Empty);

        public MyDb(BlazorDexieOptions blazorDexieOptions)
            : base("MyDatabase", 2, new IDbVersion[] { new Version1() }, blazorDexieOptions)
        {
        }
    }
}
