using DexieWrapper.Database;

namespace DexieWrapper.Test.Databases
{
    public class Version1 : DbVersion
    {
        public Store<TestItem> TestItems { get; set; } = new("uuid");

        public Version1() : base(1)
        {
        }
    }
}
