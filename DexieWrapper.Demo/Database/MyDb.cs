using Nosthy.Blazor.DexieWrapper.Database;
using Nosthy.Blazor.DexieWrapper.Demo.Persons;
using Nosthy.Blazor.DexieWrapper.Demo.TestsItems;
using Nosthy.Blazor.DexieWrapper.JsModule;

namespace Nosthy.Blazor.DexieWrapper.Demo.Database
{
    public class MyDb : Db
    {
        public Store<Person, Guid> Persons { get; set; } = new(nameof(Person.Id));
        public Store<ImageDataItem, Guid> ImageDataItems { get; set; } = new(nameof(ImageDataItem.Id));

        public MyDb(IModuleFactory jsModuleFactory)
            : base("MyDatabase", 2, new DbVersion[] { new Version1() }, jsModuleFactory)
        {
        }
    }
}
