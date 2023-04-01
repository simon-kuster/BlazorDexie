using BlazorDexie.Database;

namespace BlazorDexie.Test.V1
{
    public class Version1 : DbVersion
    {
        public Store<Friend1, int> Friends { get; set; } = new("++" + nameof(Friend1.Id), nameof(Friend1.Name), nameof(Friend1.Age));

        public Version1() : base(1)
        {
        }
    }
}
