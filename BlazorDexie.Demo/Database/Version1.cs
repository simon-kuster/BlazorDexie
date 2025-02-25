using BlazorDexie.Database;

namespace BlazorDexie.Demo.Database
{
    public class Version1 : DbVersion<Version1>
    {
        public Version1() : base(1)
        {
        }
    }
}
