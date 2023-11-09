using BlazorDexie.Database;
using BlazorDexie.JsModule;

namespace BlazorDexie.Test.V3
{
    public class Db3 : Db
    {
        public Store<Friend3, int> Friends { get; set; } = new("++" + nameof(Friend3.Id), nameof(Friend3.Name), nameof(Friend3.BirthDate));

        public Db3(IModuleFactory jsModuleFactory, string databaseId)
            : base($"VersioningDb{databaseId}", 3, new DbVersion[] { new V2.Version2(), new V1.Version1() }, jsModuleFactory, 
                  upgradeModule: "scripts/dbUpgrade3.mjs", camelCaseStoreNames: true)
        {
        }
    }
}
