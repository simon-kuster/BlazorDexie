using BlazorDexie.Database;
using BlazorDexie.JsModule;
using BlazorDexie.Test.TestItems;
using System;

namespace BlazorDexie.Test.Database
{
    public class MyDb : Db<MyDb>
    {
        public static string CompoundIndex = new CompoundIndex(nameof(TestItemWithCompoundIndex.Firstname), nameof(TestItemWithCompoundIndex.Secondname)).ToString();

        public Store<TestItem, Guid> TestItems { get; set; } = new(nameof(TestItem.Id), nameof(TestItem.Year), nameof(TestItem.Name));

        public Store<TestItemHiddenKey, Guid> TestItemsHiddenKey { get; set; } = new(string.Empty, nameof(TestItemHiddenKey.Year));

        public Store<TestItemWithCompoundIndex, int> TestItemsWithCompoundIndex { get; set; } = new(
             "++" + nameof(TestItemWithCompoundIndex.Id),
             CompoundIndex);

        public Store<byte[], Guid> BlobData { get; set; } = new(string.Empty);

        public MyDb(IModuleFactory jsModuleFactory, string databaseId)
            : base($"MyDatabase_{databaseId}", 1, Array.Empty<DbVersion>(), jsModuleFactory, camelCaseStoreNames: true)
        {
        }
    }
}
