using Jering.Javascript.NodeJS;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Nosthy.Blazor.DexieWrapper.Test.Database;
using Nosthy.Blazor.DexieWrapper.Test.ModuleWrappers;
using Nosthy.Blazor.DexieWrapper.Test.TestsItems;
using Nosthy.Blazor.DexieWrapper.Blob;
using Nosthy.Blazor.DexieWrapper.ObjUrl;

namespace Nosthy.Blazor.DexieWrapper.Test
{
    public class StoreDataTest : IAsyncLifetime
    {
        private CommonJsModuleFactory _moduleFactory;
        private ObjectUrlService _objectUrlService;

        public StoreDataTest(INodeJSService nodeJSService)
        {
            _moduleFactory = new CommonJsModuleFactory(nodeJSService, "../../../DexieWrapper/wwwroot");
            _objectUrlService = new ObjectUrlService(_moduleFactory);
        }

        [Theory]
        [InlineData(BlobDataFormat.ObjectUrl)]
        [InlineData(BlobDataFormat.ByteArray)]
        public async Task Add(BlobDataFormat blobDataFormat)
        {
            // arrange
            await using var db = CreateDb();

            // act
            var testItem = new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 10, 232 }, blobDataFormat) };
            var key = await db.TestDataItems.Add(testItem);

            // assert
            var checkItem = await db.TestDataItems.Get(testItem.Id, blobDataFormat: blobDataFormat);

            Assert.NotNull(checkItem);
            Assert.Equal(new byte[] { 123, 10, 232 }, await GetData(checkItem!.Data));
            Assert.Equal(testItem.Id, key);
        }

        [Theory]
        [InlineData(BlobDataFormat.ObjectUrl)]
        [InlineData(BlobDataFormat.ByteArray)]
        public async Task BulkAdd(BlobDataFormat blobDataFormat)
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestDataItem[]
            {
                new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 32, 232 }, blobDataFormat) },
                new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 32, 233 }, blobDataFormat) },
                new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 32, 234 }, blobDataFormat) },
                new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 32, 235 }, blobDataFormat) }
            };

            // act
            var lastkey = await db.TestDataItems.BulkAdd(initialItems);

            // assert
            var testItems = await db.TestDataItems.BulkGet(new Guid[] { initialItems[0].Id, initialItems[1].Id, initialItems[2].Id, initialItems[3].Id }, 
                blobDataFormat: blobDataFormat);

            Assert.Equal(initialItems[3].Id, lastkey);
            Assert.NotNull(testItems);
            Assert.Equal(4, testItems.Count());
            Assert.Equal(new byte[] { 123, 32, 232 }, await GetData(testItems![0]!.Data));
            Assert.Equal(new byte[] { 123, 32, 233 }, await GetData(testItems![1]!.Data));
            Assert.Equal(new byte[] { 123, 32, 234 }, await GetData(testItems![2]!.Data));
            Assert.Equal(new byte[] { 123, 32, 235 }, await GetData(testItems![3]!.Data));
        }

        [Theory]
        [InlineData(BlobDataFormat.ObjectUrl)]
        [InlineData(BlobDataFormat.ByteArray)]
        public async Task BulkGet(BlobDataFormat blobDataFormat)
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestDataItem[]
            {
                new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 33, 232 }, blobDataFormat) },
                new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 33, 233 }, blobDataFormat) },
                new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 33, 234 }, blobDataFormat) },
                new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 33, 235 }, blobDataFormat) }
            };

            await db.TestDataItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestDataItems.BulkGet(new Guid[] { initialItems[0].Id, initialItems[3].Id }, blobDataFormat: blobDataFormat);

            // assert
            Assert.NotNull(testItems);
            Assert.Equal(2, testItems.Count());
            Assert.Equal(new byte[] { 123, 33, 232 }, await GetData(testItems![0]!.Data));
            Assert.Equal(new byte[] { 123, 33, 235 }, await GetData(testItems![1]!.Data));
        }


        [Theory]
        [InlineData(BlobDataFormat.ObjectUrl)]
        [InlineData(BlobDataFormat.ByteArray)]
        public async Task BulkPut(BlobDataFormat blobDataFormat)
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestDataItem[]
            {
                new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 34, 232 }, blobDataFormat) },
                new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 34, 233 }, blobDataFormat) },
                new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 34, 234 }, blobDataFormat) },
                new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 34, 235 }, blobDataFormat) }
            };

            // act
            var lastkey = await db.TestDataItems.BulkPut(initialItems);

            // assert
            var testItems = await db.TestDataItems.BulkGet(new Guid[] { initialItems[0].Id, initialItems[1].Id, initialItems[2].Id, initialItems[3].Id }, 
                blobDataFormat: blobDataFormat);

            Assert.Equal(initialItems[3].Id, lastkey);
            Assert.NotNull(testItems);
            Assert.Equal(4, testItems.Count());
            Assert.Equal(new byte[] { 123, 34, 232 }, await GetData(testItems![0]!.Data));
            Assert.Equal(new byte[] { 123, 34, 233 }, await GetData(testItems![1]!.Data));
            Assert.Equal(new byte[] { 123, 34, 234 }, await GetData(testItems![2]!.Data));
            Assert.Equal(new byte[] { 123, 34, 235 }, await GetData(testItems![3]!.Data));
        }

        [Theory]
        [InlineData(BlobDataFormat.ObjectUrl)]
        [InlineData(BlobDataFormat.ByteArray)]
        public async Task Get(BlobDataFormat blobDataFormat)
        {
            // arrange
            await using var db = CreateDb();

            var initialItem = new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 40, 232 }, blobDataFormat) };
            await db.TestDataItems.Put(initialItem);

            // act
            var testItem = await db.TestDataItems.Get(initialItem.Id, blobDataFormat: blobDataFormat);

            // assert
            Assert.NotNull(testItem);
            Assert.Equal(new byte[] { 123, 40, 232 }, await GetData(testItem!.Data));
        }

        [Theory]
        [InlineData(BlobDataFormat.ObjectUrl)]
        [InlineData(BlobDataFormat.ByteArray)]
        public async Task Put(BlobDataFormat blobDataFormat)
        {
            // arrange
            await using var db = CreateDb();

            // act
            var testItem = new TestDataItem() { Id = Guid.NewGuid(), Data = await CreateBlobData(new byte[] { 123, 41, 232 }, blobDataFormat) };
            var key = await db.TestDataItems.Put(testItem);

            // assert
            var checkItem = await db.TestDataItems.Get(testItem.Id, blobDataFormat: blobDataFormat);

            Assert.Equal(key, testItem.Id);
            Assert.NotNull(checkItem);
            Assert.Equal(new byte[] { 123, 41, 232 }, await GetData(checkItem!.Data));
        }

        [Theory]
        [InlineData(BlobDataFormat.ObjectUrl)]
        [InlineData(BlobDataFormat.ByteArray)]
        public async Task Update(BlobDataFormat blobDataFormat)
        {
            // arrange
            await using var db = CreateDb();

            var initialItem = new TestDataItem() { Id = Guid.NewGuid(), Data = await  CreateBlobData(new byte[] { 123, 42, 232 }, blobDataFormat) };
            await db.TestDataItems.Put(initialItem);

            // act
            var updatedRecord = await db.TestDataItems.Update(initialItem.Id, new Dictionary<string, object> { { nameof(TestItem.Name), "CC" } });

            // assert
            var checkItem = await db.TestDataItems.Get(initialItem.Id, blobDataFormat: blobDataFormat);

            Assert.Equal(1, updatedRecord);
            Assert.NotNull(checkItem);
            Assert.Equal(new byte[] { 123, 42, 232 }, await GetData(checkItem!.Data));
        }

        private MyDb CreateDb()
        {
            var databaseId = Guid.NewGuid().ToString();
            return new MyDb(_moduleFactory, databaseId);
        }

        private async Task<BlobData> CreateBlobData(byte[] data, BlobDataFormat blobDataFormat)
        {
            byte[]? byteArray = null;
            string? objectUrl = null;

            switch (blobDataFormat)
            {
                case BlobDataFormat.ObjectUrl:
                    objectUrl = await _objectUrlService.Create(data);
                    break;

                case BlobDataFormat.ByteArray:
                    byteArray = data;
                    break;

                default:
                    throw new ArgumentException(nameof(blobDataFormat));
            }

            return new BlobData(byteArray, objectUrl);
        }

        private async Task<byte[]> GetData(BlobData blobData)
        {
            if (blobData.ByteArray != null)
            {
                return blobData.ByteArray;
            }

            if (blobData.ObjectUrl != null)
            {
                var byteArray =  await _objectUrlService.FetchDataNode(blobData.ObjectUrl);
                await _objectUrlService.Revoke(blobData.ObjectUrl);
                return byteArray;
            }

            return new byte[0];
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _objectUrlService.DisposeAsync().ConfigureAwait(false);
        }
    }
}
