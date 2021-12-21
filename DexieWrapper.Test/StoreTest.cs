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
        public async Task BulkGet()
        {
            // arrange
            var moduleFactory = new ModuleWrapperFactory(_nodeJSService, "../../DexieWrapper/wwwroot");
            var db = new MyDb(moduleFactory);

            TestItem[] initialItems = new TestItem[4] 
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA" }, 
                new TestItem() { Id = Guid.NewGuid(), Name = "BB" }, 
                new TestItem() { Id = Guid.NewGuid(), Name = "CC" }, 
                new TestItem() { Id = Guid.NewGuid(), Name = "DD" } 
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.BulkGet(new object[4] { initialItems[0].Id, initialItems[1].Id, initialItems[2].Id, initialItems[3].Id });

            // assert
            Assert.NotNull(testItems);

            for (int i = 0; i < testItems!.Length; i++)
            {
                Assert.NotNull(testItems[i]);
                Assert.Equal(initialItems[i].Name, testItems![i]!.Name);
            }
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
        public async Task BulkPut()
        {
            // arrange
            var moduleFactory = new ModuleWrapperFactory(_nodeJSService, "../../DexieWrapper/wwwroot");
            var db = new MyDb(moduleFactory);

            TestItem[] initialItems = new TestItem[4]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA" },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB" },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC" },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD" }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.BulkGet(new object[4] { initialItems[0].Id, initialItems[1].Id, initialItems[2].Id, initialItems[3].Id });

            // assert
            Assert.NotNull(testItems);

            for (int i = 0; i < testItems!.Length; i++)
            {
                Assert.NotNull(testItems[i]);
                Assert.Equal(initialItems[i].Name, testItems![i]!.Name);
            }
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

        [Fact]
        public async Task BulkDelete()
        {
            // arrange
            var moduleFactory = new ModuleWrapperFactory(_nodeJSService, "../../DexieWrapper/wwwroot");
            var db = new MyDb(moduleFactory);

            TestItem[] initialItems = new TestItem[4]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA" },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB" },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC" },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD" }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            await db.TestItems.BulkDelete(new object[4] { initialItems[0].Id, initialItems[1].Id, initialItems[2].Id, initialItems[3].Id });

            // assert
            var testItems = await db.TestItems.BulkGet(new object[4] { initialItems[0].Id, initialItems[1].Id, initialItems[2].Id, initialItems[3].Id });

            for (int i = 0; i < testItems!.Length; i++)
            {
                Assert.True(testItems[i] == null);
            }
        }

        [Fact]
        public async Task ToArray()
        {
            // arrange
            var moduleFactory = new ModuleWrapperFactory(_nodeJSService, "../../DexieWrapper/wwwroot");
            var db = new MyDb(moduleFactory);

            TestItem[] initialItems = new TestItem[4]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA" },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB" },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC" },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD" }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.ToArray();

            // assert
            Assert.NotNull(testItems);

            int matches = 0;
            for (int i = 0; i < testItems!.Length; i++)
            {
                Assert.NotNull(testItems[i]);

                for (int j = 0; j < testItems!.Length; j++)
                {
                    if (testItems![i]!.Name == initialItems[j].Name)
                    {
                        matches += 1;
                    }
                }
            }

            Assert.True(matches == testItems.Length);
        }

        [Fact]
        public async Task Where()
        {
            // arrange
            var moduleFactory = new ModuleWrapperFactory(_nodeJSService, "../../DexieWrapper/wwwroot");
            var db = new MyDb(moduleFactory);

            TestItem[] initialItems = new TestItem[4]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2010 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2012 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2012 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2015 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where("year").IsEqual(2012).ToArray();

            // assert
            Assert.Equal(2, testItems?.Length);
        }
    }
}
