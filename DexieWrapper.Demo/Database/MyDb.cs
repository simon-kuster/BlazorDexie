using DexieWrapper.Database;
using DexieWrapper.Demo.Persons;
using DexieWrapper.JsModule;

namespace DexieWrapper.Demo.Database
{
    public class MyDb : Db
    {
        public Store<Person, Guid> Persons { get; set; } = new(nameof(Person.Id));

        public MyDb(IJsModuleFactory jsModuleFactory) 
            : base("MyDatabase", 2, new DbVersion[] { new Version1() }, jsModuleFactory)
        {
        }
    }
}
