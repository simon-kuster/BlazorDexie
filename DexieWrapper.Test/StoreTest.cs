using DexieWrapper.Test.Databases;
using DexieWrapper.Test.ModuleWrappers;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DexieWrapper.Test
{
    public class StoreTest
    {
        private INodeJSService _nodeJSService;

        public StoreTest()
        {
            var services = new ServiceCollection();
            services.AddNodeJS();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            _nodeJSService = serviceProvider.GetRequiredService<INodeJSService>();
        }

        [Fact]
        public async Task Get()
        {
            // arrange
            var moduleFactory = new ModuleWrapperFactory(_nodeJSService, "../../DexieWrapper/wwwroot");
            var db = new MyDb(moduleFactory);

            TestItem initialItem = new TestItem() { Id = Guid.NewGuid(), Name = "AA" };
            await db.TestItems.Put(initialItem);

            // act
            var testItem = await db.TestItems.Get(initialItem.Id);

            // assert
            Assert.NotNull(testItem);
            Assert.Equal("AA", testItem!.Name);
        }

        [Fact]
        public async Task Put()
        {
            // arrange
            var moduleFactory = new ModuleWrapperFactory(_nodeJSService, "../../DexieWrapper/wwwroot");
            var db = new MyDb(moduleFactory);

            // act
            var testItem = new TestItem() { Id = Guid.NewGuid(), Name = "BB" };
            await db.TestItems.Put(testItem);

            // assert
            var checkItem = await db.TestItems.Get(testItem.Id);
            
            Assert.NotNull(checkItem);
            Assert.Equal("BB", checkItem!.Name);
        }

        [Fact]
        public async Task Delete()
        {
            // arrange
            var moduleFactory = new ModuleWrapperFactory(_nodeJSService, "../../DexieWrapper/wwwroot");
            var db = new MyDb(moduleFactory);

            TestItem initialItem = new TestItem() { Id = Guid.NewGuid(), Name = "AA" };
            await db.TestItems.Put(initialItem);

            // act
            await db.TestItems.Delete(initialItem.Id);

            // assert
            Assert.True(await db.TestItems.Get(initialItem.Id) == null);
        }
    }
}
