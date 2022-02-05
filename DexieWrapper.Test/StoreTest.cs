using DexieWrapper.Test.Databases;
using DexieWrapper.Test.ModuleWrappers;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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
            var db = CreateDb();

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
            var testItems = await db.TestItems.BulkGet(new Guid[4] { initialItems[0].Id, initialItems[1].Id, initialItems[2].Id, initialItems[3].Id });

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
            var db = CreateDb();

            // act
            var testItem = new TestItem() { Id = Guid.NewGuid(), Name = "BB" };
            var key = await db.TestItems.Put(testItem);

            // assert
            var checkItem = await db.TestItems.Get(testItem.Id);

            Assert.NotNull(checkItem);
            Assert.Equal("BB", checkItem!.Name);
            Assert.Equal(testItem.Id, key);
        }

        [Fact]
        public async Task BulkPut()
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

            // act
            await db.TestItems.BulkPut(initialItems);

            // assert
            var testItems = await db.TestItems.BulkGet(new Guid[4] { initialItems[0].Id, initialItems[1].Id, initialItems[2].Id, initialItems[3].Id });
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
            var db = CreateDb();

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
            await db.TestItems.BulkDelete(new Guid[4] { initialItems[0].Id, initialItems[1].Id, initialItems[2].Id, initialItems[3].Id });

            // assert
            var testItems = await db.TestItems.BulkGet(new Guid[4] { initialItems[0].Id, initialItems[1].Id, initialItems[2].Id, initialItems[3].Id });

            for (int i = 0; i < testItems!.Length; i++)
            {
                Assert.True(testItems[i] == null);
            }
        }


        [Fact]
        public async Task Clear()
        {
            // arrange
            var db = CreateDb();

            TestItem initialItem = new TestItem() { Id = Guid.NewGuid(), Name = "AA" };
            await db.TestItems.Put(initialItem);

            // act
            await db.TestItems.Clear();

            // assert
            Assert.True((await db.TestItems.ToArray()).Length == 0);
        }

        private MyDb CreateDb()
        {
            var moduleFactory = new ModuleWrapperFactory(_nodeJSService, "../../../DexieWrapper/wwwroot");
            return new MyDb(moduleFactory);
        }

        [Fact]
        public async Task Where()
        {
            // arrange
            var db = CreateDb();

            TestItem[] initialItems = new TestItem[4]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2010 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2012 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2012 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2015 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(new Dictionary<string, object>
            {
                { nameof(TestItem.Year), 2015 }
            }).ToArray();

            // assert
            Assert.Equal(1, testItems?.Length);
            Assert.Equal(initialItems[3].Id, testItems?.First()?.Id);
        }
    }
}
