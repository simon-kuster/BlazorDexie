using Nosthy.Blazor.DexieWrapper.Database;
using Nosthy.Blazor.DexieWrapper.JsModule;
using Nosthy.Blazor.DexieWrapper.Test.TestsItems;
using System;

namespace Nosthy.Blazor.DexieWrapper.Test.Database
{
    public class MyDb : Db
    {
        public Store<TestItem, Guid> TestItems { get; set; } = new(nameof(TestItem.Id), nameof(TestItem.Year), nameof(TestItem.Name));
        
        public Store<TestItemHiddenKey, Guid> TestItemsHiddenKey { get; set; } = new(string.Empty, nameof(TestItemHiddenKey.Year));

        public Store<TestItemWithCompoundIndex, int> TestItemsWithCompoundIndex { get; set; } = new(
             "++" + nameof(TestItemWithCompoundIndex.Id),
            new CompoundIndex(nameof(TestItemWithCompoundIndex.Firstname), nameof(TestItemWithCompoundIndex.Secondname)).ToString());

        public Store<TestDataItem, Guid> TestDataItems { get; set; } = new(nameof(TestDataItem.Id));

        public MyDb(IModuleFactory jsModuleFactory)
            : base("MyDatabase", 2, new DbVersion[] { new Version1() }, jsModuleFactory)
        {
        }
    }
}
