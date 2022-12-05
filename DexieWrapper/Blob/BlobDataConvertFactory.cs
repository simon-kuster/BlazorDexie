using Nosthy.Blazor.DexieWrapper.Utils;

namespace Nosthy.Blazor.DexieWrapper.Blob
{
    public class BlobDataConvertFactory<T>
    {
        private List<string> pathes;

        public BlobDataConvertFactory()
        {
            pathes = FindBlobDataPathesInternal(typeof(T));
        }

        public BlobDataConvert CreatetForWrite(int parameterIndex, bool isEnumerable)
        {
            var pathBeginn = isEnumerable ? $"[{parameterIndex}]*." : $"[{parameterIndex}].";
            var pathesForWrite = pathes.Select(p => $"{pathBeginn}{p}").ToList();
            return new BlobDataConvert(pathesForWrite, BlobDataConvertType.ToBlob);
        }

        public BlobDataConvert CreateForRead(bool isEnumerable, BlobDataFormat blobDataFormat)
        {
            var pathBeginn = isEnumerable ?  "*." : ".";
            var pathesForRead = pathes.Select(p => $"{pathBeginn}{p}").ToList();

            BlobDataConvertType converType;
            switch (blobDataFormat)
            {
                case BlobDataFormat.ByteArray:
                    converType = BlobDataConvertType.ToByteArray;
                    break;

                case BlobDataFormat.ObjectUrl:
                    converType = BlobDataConvertType.ToObjectUrl;
                    break;

                default:
                    throw new InvalidOperationException($"{GetType().Name}: Unsupported BlobDataFormat {blobDataFormat}");
            }

            return new BlobDataConvert(pathesForRead, converType);
        }

        private List<string> FindBlobDataPathesInternal(Type type, string? prefix = null)
        {
            var properties = type.GetProperties();

            var pathes = new List<string>();

            foreach (var property in properties)
            {
                var path = CombinePath(prefix, Camelizer.ToCamelCase(property.Name));

                if (property.PropertyType == typeof(BlobData))
                {
                    pathes.Add(path);
                }
            }

            return pathes;
        }

        private string CombinePath(string? path1, string path2)
        {
            if (string.IsNullOrEmpty(path1))
            {
                return path2;
            }

            return $"{path1}.{path2}";
        }
    }
}
