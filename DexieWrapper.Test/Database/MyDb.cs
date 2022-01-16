using DexieWrapper.Database;
using DexieWrapper.JsModule;

namespace DexieWrapper.Test.Databases
{
    public class MyDb : Db
    {
        public Store<TestItem> TestItems { get; set; } = new(nameof(TestItem.Id), nameof(TestItem.Year));

        public MyDb(IJsModuleFactory jsModuleFactory) 
            : base("MyDatabase", 2, new DbVersion[] { new Version1() }, jsModuleFactory)
        {
        }
    }
}
