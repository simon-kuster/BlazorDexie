using Nosthy.Blazor.DexieWrapper.Database;
using Nosthy.Blazor.DexieWrapper.Test.TestsItems;
using System;

namespace Nosthy.Blazor.DexieWrapper.Test.Database
{
    public class Version1 : DbVersion
    {
        public Store<TestItem, Guid> TestItems { get; set; } = new("Uuid");

        public Version1() : base(1)
        {
        }
    }
}
