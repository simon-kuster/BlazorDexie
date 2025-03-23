using BlazorDexie.Database;
using BlazorDexie.Options;

namespace BlazorDexie.Test.V3
{
    public class Db3 : Db<Db3>
    {
        public Store<Friend3, int> Friends { get; set; } = new("++" + nameof(Friend3.Id), nameof(Friend3.Name), nameof(Friend3.BirthDate));

        public Db3(BlazorDexieOptions blazorDexieOptions, string databaseId)
            : base($"VersioningDb{databaseId}", 3, new IDbVersion[] { new V2.Version2(), new V1.Version1() }, blazorDexieOptions, 
                  upgradeModule: "scripts/dbUpgrade3.mjs")
        {
        }
    }
}
