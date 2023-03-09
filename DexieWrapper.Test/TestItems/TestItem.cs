using System;

namespace BlazorDexie.Test.TestItems
{
    public class TestItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Year { get; set; }
    }
}
