using Nosthy.Blazor.DexieWrapper.Database;
using Nosthy.Blazor.DexieWrapper.JsModule;
using Nosthy.Blazor.DexieWrapper.Test;
using System;

namespace Nosthy.Blazor.DexieWrapper.Test.Database
{
    public class MyDb : Db
    {
        public Store<TestItem, Guid> TestItems { get; set; } = new(nameof(TestItem.Id), nameof(TestItem.Year), nameof(TestItem.Name));
        public Store<TestItemHiddenKey, Guid> TestItemsHiddenKey { get; set; } = new(string.Empty, nameof(TestItem.Id), nameof(TestItem.Year));

        public MyDb(IModuleFactory jsModuleFactory)
            : base("MyDatabase", 2, new DbVersion[] { new Version1() }, jsModuleFactory)
        {
        }
    }
}
