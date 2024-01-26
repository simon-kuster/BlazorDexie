using System.Threading.Tasks;
using Xunit;
using System;
using BlazorDexie.Test.Database;
using BlazorDexie.ObjUrl;
using BlazorDexie.JsModule;
using System.Linq;

namespace BlazorDexie.Test
{
    public class StoreDataTest : IAsyncLifetime
    {
        private IModuleFactory _moduleFactory;
        private ObjectUrlService _objectUrlService;


        public StoreDataTest(IModuleFactory moduleFactory, ObjectUrlService objectUrlService)
        {
            _moduleFactory = moduleFactory;
            _objectUrlService = objectUrlService;
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task AddBlob()
        {
            // arrange
            await using var db = CreateDb();

            // act
            var initialData = new byte[] { 213, 23, 55, 234, 54 };
            var key = Guid.NewGuid();
            await db.BlobData.AddBlob(initialData, key);

            // assert
            var data = await db.BlobData.GetBlob(key);

            Assert.NotNull(data);
            Assert.Equal(initialData, data);
        }

        [Fact]
        public async Task AddObjectUrl()
        {
            // arrange
            await using var db = CreateDb();

            // act
            var initialData = new byte[] { 213, 23, 55, 234, 54 };
            var initialObjectUrl = await _objectUrlService.Create(initialData);
            var key = Guid.NewGuid();
            await db.BlobData.AddObjectUrl(initialObjectUrl, key);

            // assert
            var objectUrl = await db.BlobData.GetObjectUrl(key);
            var data = await _objectUrlService.FetchData(objectUrl);

            Assert.NotNull(data);
            Assert.Equal(initialData, data);
        }

        [Fact]
        public async Task PutBlob()
        {
            // arrange
            await using var db = CreateDb();

            // act
            var initalData = new byte[] { 213, 23, 55, 234, 54 };
            var key = Guid.NewGuid();
            await db.BlobData.PutBlob(initalData, key);

            // assert
            var data = await db.BlobData.GetBlob(key);

            Assert.NotNull(data);
            Assert.Equal(initalData, data);
        }

        [Fact]
        public async Task PutObjectUrl()
        {
            // arrange
            await using var db = CreateDb();

            // act
            var initialData = new byte[] { 213, 23, 55, 234, 54 };
            var initialObjectUrl = await _objectUrlService.Create(initialData);
            var key = Guid.NewGuid();
            await db.BlobData.PutObjectUrl(initialObjectUrl, key);

            // assert
            var objectUrl = await db.BlobData.GetObjectUrl(key);
            var data = await _objectUrlService.FetchData(objectUrl);

            Assert.NotNull(data);
            Assert.Equal(initialData, data);
        }

        [Fact]
        public async Task GetBlob()
        {
            // arrange
            await using var db = CreateDb();

            var initalData = new byte[] { 213, 23, 55, 234, 54 };
            var key = Guid.NewGuid();
            await db.BlobData.PutBlob(initalData, key);

            // act
            var data = await db.BlobData.GetBlob(key);

            // assert
            Assert.NotNull(data);
            Assert.Equal(initalData, data);
        }

        [Fact]
        public async Task GetObjectUrl()
        {
            // arrange
            await using var db = CreateDb();

            var initalData = new byte[] { 213, 28, 55, 234, 54 };
            var key = Guid.NewGuid();
            await db.BlobData.PutBlob(initalData, key);

            // act
            var objectUrl = await db.BlobData.GetObjectUrl(key);

            // assert
            var data = await _objectUrlService.FetchData(objectUrl);

            Assert.NotNull(data);
            Assert.Equal(initalData, data);
        }

        [Fact]
        public async Task GetPrimaryKeys()
        {
            // arrange
            await using var db = CreateDb();

            var initalData = new byte[] { 213, 28, 55, 234, 54 };
            var key = Guid.NewGuid();
            await db.BlobData.PutBlob(initalData, key);

            var initalData2 = new byte[] { 213, 28, 55, 234, 54 };
            var key2 = Guid.NewGuid();
            await db.BlobData.PutBlob(initalData2, key2);

            // act
            var primaryKeys = await db.BlobData.PrimaryKeys();

            // assert
            Assert.Equal(2, primaryKeys.Length);
            Assert.Contains(key, primaryKeys);
            Assert.Contains(key2, primaryKeys);
        }

        [Fact]
        public async Task BulkAddBlob()
        {
            // arrange
            await using var db = CreateDb();

            // act
            var initialData = new byte[][] { [213, 23, 55, 234, 54], [23, 23, 44], [11, 22, 33] };
            var keys = Enumerable.Range(0, initialData.Length).Select(_ => Guid.NewGuid()).ToArray();

#pragma warning disable CS0618 // Type or member is obsolete
            await db.BlobData.BulkAddBlob(initialData, keys);
#pragma warning restore CS0618 // Type or member is obsolete

            // assert
            var data = await Task.WhenAll(keys.Select(async k => await db.BlobData.GetBlob(k)));

            Assert.NotNull(data);

            for (int i = 0; i < 2;  i++)
            {
                Assert.Equal(initialData[i], data[i]);
            }
        }

        private MyDb CreateDb()
        {
            var databaseId = Guid.NewGuid().ToString();
            return new MyDb(_moduleFactory, databaseId);
        }

        public async Task DisposeAsync()
        {
            await _objectUrlService.DisposeAsync().ConfigureAwait(false);
        }
    }
}
