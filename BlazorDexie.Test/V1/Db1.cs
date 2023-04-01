using BlazorDexie.Database;
using BlazorDexie.JsModule;

namespace BlazorDexie.Test.V1
{
    public class Db1 : Db
    {
        public Store<Friend1, int> Friends { get; set; } = new("++" + nameof(Friend1.Id), nameof(Friend1.Name), nameof(Friend1.Age));

        public Db1(IModuleFactory jsModuleFactory, string databaseId)
            : base($"VersioningDb{databaseId}", 1, new DbVersion[0], jsModuleFactory)
        {
        }
    }
}
