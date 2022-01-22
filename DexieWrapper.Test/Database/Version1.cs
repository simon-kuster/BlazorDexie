using DexieWrapper.Database;
using System;

namespace DexieWrapper.Test.Databases
{
    public class Version1 : DbVersion
    {
        public Store<TestItem, Guid> TestItems { get; set; } = new("Uuid");

        public Version1() : base(1)
        {
        }
    }
}
