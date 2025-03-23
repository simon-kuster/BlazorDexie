using BlazorDexie.Database;
using BlazorDexie.Options;
namespace BlazorDexie.Test.V2
{
    public class Db2 : Db<Db2>
    {
        public Store<Friend2, int> Friends { get; set; } = new("++" + nameof(Friend2.Id), nameof(Friend2.Name), nameof(Friend2.BirthDate));

        public Db2(BlazorDexieOptions blazorDexieOptions, string databaseId)
            : base($"VersioningDb{databaseId}", 2, new IDbVersion[] { new V1.Version1() }, blazorDexieOptions, GetUpgrade())
        {
        }

        private static string GetUpgrade()
        {
            return
                "var YEAR = 365 * 24 * 60 * 60 * 1000; " +
                "return tx.table(\"Friends\").toCollection().modify(friend => { " +
                "    friend.birthdate = new Date(Date.now() - (friend.age * YEAR)); " +
                "    delete friend.age; " +
                "}); ";

        }
    }
}
