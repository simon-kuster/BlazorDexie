using BlazorDexie.Database;
using BlazorDexie.Demo.Persons;
using BlazorDexie.JsModule;

namespace BlazorDexie.Demo.Database
{
    public class MyDb : Db<MyDb>
    {
        public Store<Person, Guid> Persons { get; set; } = new(nameof(Person.Id));

        public Store<byte[], Guid> BlobData { get; set; } = new(string.Empty);

        public MyDb(IModuleFactory jsModuleFactory)
            : base("MyDatabase", 2, new DbVersion[] { new Version1() }, jsModuleFactory, camelCaseStoreNames : true)
        {
        }
    }
}
