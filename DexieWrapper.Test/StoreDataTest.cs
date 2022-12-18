using Jering.Javascript.NodeJS;
using System.Threading.Tasks;
using Xunit;
using Nosthy.Blazor.DexieWrapper.Test.ModuleWrappers;
using Nosthy.Blazor.DexieWrapper.ObjUrl;
using System;
using Nosthy.Blazor.DexieWrapper.Test.Database;

namespace Nosthy.Blazor.DexieWrapper.Test
{
    public class StoreDataTest : IAsyncLifetime
    {
        private INodeJSService _nodeJSService;
        private ObjectUrlService _objectUrlService;

        public StoreDataTest(INodeJSService nodeJSService)
        {
            _nodeJSService = nodeJSService;
            var moduleFactory = new CommonJsModuleFactory(_nodeJSService, "../../../DexieWrapper/wwwroot");
            _objectUrlService = new ObjectUrlService(moduleFactory);
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        //[Fact]
        //public async Task AddBlob()
        //{
        //    // arrange
        //    await using var db = CreateDb();

        //    // act
        //    var initialData = new byte[] { 213, 23, 55, 234, 54 };
        //    var key = Guid.NewGuid();
        //    await db.BlobData.AddBlob(initialData, key);

        //    // assert
        //    var data = await db.BlobData.GetBlob(key);

        //    Assert.NotNull(data);
        //    Assert.Equal(initialData, data);
        //}

        //[Fact]
        //public async Task PutBlob()
        //{
        //    // arrange
        //    await using var db = CreateDb();

        //    // act
        //    var initalData = new byte[] { 213, 23, 55, 234, 54 };
        //    var key = Guid.NewGuid();
        //    await db.BlobData.PutBlob(initalData, key);

        //    // assert
        //    var data = await db.BlobData.GetBlob(key);

        //    Assert.NotNull(data);
        //    Assert.Equal(initalData, data);
        //}

        //[Fact]
        //public async Task GetBlob()
        //{
        //    // arrange
        //    await using var db = CreateDb();

        //    var initalData = new byte[] { 213, 23, 55, 234, 54 };
        //    var key = Guid.NewGuid();
        //    await db.BlobData.PutBlob(initalData, key);

        //    // act
        //    var data = await db.BlobData.GetBlob(key);

        //    // assert
        //    Assert.NotNull(data);
        //    Assert.Equal(initalData, data);
        //}

        private MyDb CreateDb()
        {
            var databaseId = Guid.NewGuid().ToString();
            var moduleFactory = new CommonJsModuleFactory(_nodeJSService, "../../../DexieWrapper/wwwroot");
            return new MyDb(moduleFactory, databaseId);
        }

        public async Task DisposeAsync()
        {
            await _objectUrlService.DisposeAsync().ConfigureAwait(false);
        }
    }
}
