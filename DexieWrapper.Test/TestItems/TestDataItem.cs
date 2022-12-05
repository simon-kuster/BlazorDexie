using Nosthy.Blazor.DexieWrapper.Blob;
using System;

namespace Nosthy.Blazor.DexieWrapper.Test.TestsItems
{
    public class TestDataItem
    {
        public Guid Id { get; set; }
        public BlobData Data { get; set; } = new BlobData(new byte[0]);
    }
}
