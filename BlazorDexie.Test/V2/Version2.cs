using BlazorDexie.Database;

namespace BlazorDexie.Test.V2
{
    public class Version2 : DbVersion<Version2>
    {
        public Store<Friend2, int> Friends { get; set; } = new("++" + nameof(Friend2.Id), nameof(Friend2.Name), nameof(Friend2.BirthDate));
        public Store<Friend2, int> Friends2 { get; set; } = new("++" + nameof(Friend2.Id), nameof(Friend2.Name), nameof(Friend2.BirthDate));

        public Version2() : base(2, GetUpgrade())
        {
        }

        private static string GetUpgrade()
        {
            return
                "var YEAR = 365 * 24 * 60 * 60 * 1000; " +
                "return tx.table(\"friends\").toCollection().modify(friend => { " +
                "    friend.birthdate = new Date(Date.now() - (friend.age * YEAR)); " +
                "    delete friend.age; " +
                "}); ";

        }
    }
}
