namespace Nosthy.Blazor.DexieWrapper.Blob
{
    public class BlobDataConvert
    {
        public IEnumerable<string> Pathes { get; }
        public BlobDataConvertType ConvertType { get; }

        public BlobDataConvert(IEnumerable<string> pathes, BlobDataConvertType converType)
        {
            Pathes = pathes;
            ConvertType = converType;
        }
    }
}
