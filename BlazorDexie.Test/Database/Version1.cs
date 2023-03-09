using BlazorDexie.Database;
using BlazorDexie.Test.TestItems;
using System;

namespace BlazorDexie.Test.Database
{
    public class Version1 : DbVersion
    {
        public Store<TestItem, Guid> TestItems { get; set; } = new("Uuid");

        public Version1() : base(1)
        {
        }
    }
}
