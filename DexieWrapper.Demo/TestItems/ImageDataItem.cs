using Nosthy.Blazor.DexieWrapper.Blob;

namespace Nosthy.Blazor.DexieWrapper.Demo.TestsItems
{
    public class ImageDataItem
    {
        public Guid Id { get; set; }
        public BlobData Data { get; set; } = new BlobData(new byte[0]);
    }
}
