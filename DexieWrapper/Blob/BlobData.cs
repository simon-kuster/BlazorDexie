namespace Nosthy.Blazor.DexieWrapper.Blob
{
    public class BlobData
    {
        public byte[]? ByteArray { get; }
        public string? ObjectUrl { get; } 

        public BlobData(byte[]? byteArray = null, string? objectUrl = null)
        {
            if (byteArray == null && objectUrl == null)
            {
                throw new ArgumentException($"One of the two parameters {byteArray} {objectUrl} should have a value." );
            }

            if (byteArray != null && objectUrl != null)
            {
                throw new ArgumentException($"Only one of the two parameters {byteArray} {objectUrl} should have a value.");
            }

            ByteArray = byteArray;
            ObjectUrl = objectUrl;
        }
    }
}
