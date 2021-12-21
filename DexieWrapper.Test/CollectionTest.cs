using DexieWrapper.Test.Databases;
using DexieWrapper.Test.ModuleWrappers;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DexieWrapper.Test
{
    public class CollectionTest
    {
        private INodeJSService _nodeJSService;

        public CollectionTest()
        {
            var services = new ServiceCollection();
            services.AddNodeJS();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            _nodeJSService = serviceProvider.GetRequiredService<INodeJSService>();
        }

        [Fact]
        public async Task ToArrayAndToList()
        {
            // arrange
            var db = CreateDb();

            TestItem[] initialItems = new TestItem[4]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA" },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB" },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC" },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD" }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItemArray = await db.TestItems.ToArray();
            var testItemList = await db.TestItems.ToList();

            // assert
            Assert.Equal(4, testItemArray.Length);
            Assert.Equal(4, testItemList.Count);

            foreach (var initialItem in initialItems)
            {
                Assert.Equal(initialItem.Name, testItemArray.First(i => i.Id == initialItem.Id).Name);
                Assert.Equal(initialItem.Name, testItemList.First(i => i.Id == initialItem.Id).Name);
            }
        }

        private MyDb CreateDb()
        {
            var moduleFactory = new ModuleWrapperFactory(_nodeJSService, "../../DexieWrapper/wwwroot");
            return new MyDb(moduleFactory);
        }
    }
}
