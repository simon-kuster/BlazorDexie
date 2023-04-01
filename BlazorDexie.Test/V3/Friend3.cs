using System;

namespace BlazorDexie.Test.V3
{
    public class Friend3
    {
        public int? Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public string Sex { get; set; } = string.Empty;

        public int IsAdult { get; set; }
    }
}
