using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Nosthy.Blazor.DexieWrapper.Test.Database;
using Nosthy.Blazor.DexieWrapper.Test.ModuleWrappers;

namespace Nosthy.Blazor.DexieWrapper.Test
{
    public class StoreTest
    {
        private INodeJSService _nodeJSService;

        public StoreTest()
        {
            var services = new ServiceCollection();
            services.AddNodeJS();
            var serviceProvider = services.BuildServiceProvider();
            _nodeJSService = serviceProvider.GetRequiredService<INodeJSService>();
        }

        [Fact]
        public async Task Add()
        {
            // arrange
            var db = CreateDb();

            // act
            var testItem = new TestItem() { Id = Guid.NewGuid(), Name = "BB" };
            var key = await db.TestItems.Add(testItem);

            // assert
            var checkItem = await db.TestItems.Get(testItem.Id);

            Assert.NotNull(checkItem);
            Assert.Equal("BB", checkItem!.Name);
            Assert.Equal(testItem.Id, key);
        }

        [Fact]
        public async Task AddHiddenKey()
        {
            // arrange
            var db = CreateDb();

            // act
            var testItem = new TestItemHiddenKey() { Name = "BB" };
            var key = Guid.NewGuid();
            await db.TestItemsHiddenKey.Add(testItem, key);

            // assert
            var checkItem = await db.TestItemsHiddenKey.Get(key);

            Assert.NotNull(checkItem);
            Assert.Equal("BB", checkItem!.Name);
        }

        [Fact]
        public async Task BulkAdd()
        {
            // arrange
            var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA" },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB" },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC" },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD" }
            };

            // act
            var lastkey = await db.TestItems.BulkAdd(initialItems);

            // assert
            var testItems = await db.TestItems.BulkGet(new Guid[] { initialItems[0].Id, initialItems[1].Id, initialItems[2].Id, initialItems[3].Id });

            Assert.Equal(initialItems[3].Id, lastkey);
            Assert.NotNull(testItems);
            Assert.Equal(4, testItems.Count());
            Assert.Equal("AA", testItems![0]!.Name);
            Assert.Equal("BB", testItems![1]!.Name);
            Assert.Equal("CC", testItems![2]!.Name);
            Assert.Equal("DD", testItems![3]!.Name);
        }

        [Fact]
        public async Task BulkAddReturnAllkeys()
        {
            // arrange
            var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA" },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB" },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC" },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD" }
            };

            // act
            var keys = await db.TestItems.BulkAddReturnAllKeys(initialItems);

            // assert
            var testItems = await db.TestItems.BulkGet(new Guid[] { initialItems[0].Id, initialItems[1].Id, initialItems[2].Id, initialItems[3].Id });

            Assert.Equal(initialItems[0].Id, keys[0]);
            Assert.Equal(initialItems[1].Id, keys[1]);
            Assert.Equal(initialItems[2].Id, keys[2]);
            Assert.Equal(initialItems[3].Id, keys[3]);

            Assert.NotNull(testItems);
            Assert.Equal(4, testItems.Length);
            Assert.Equal("AA", testItems![0]!.Name);
            Assert.Equal("BB", testItems![1]!.Name);
            Assert.Equal("CC", testItems![2]!.Name);
            Assert.Equal("DD", testItems![3]!.Name);
        }

        [Fact]
        public async Task BulkAddHiddenKey()
        {
            // arrange
            var db = CreateDb();

            var initialItems = new TestItemHiddenKey[]
            {
                new TestItemHiddenKey() { Name = "AA" },
                new TestItemHiddenKey() { Name = "BB" },
                new TestItemHiddenKey() { Name = "CC" },
                new TestItemHiddenKey() { Name = "DD" }
            };

            var keys = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            // act
            await db.TestItemsHiddenKey.BulkAdd(initialItems, keys);

            // assert
            var testItems = await db.TestItemsHiddenKey.BulkGet(keys);

            Assert.NotNull(testItems);
            Assert.Equal("AA", testItems![0]!.Name);
            Assert.Equal("BB", testItems![1]!.Name);
            Assert.Equal("CC", testItems![2]!.Name);
            Assert.Equal("DD", testItems![3]!.Name);
        }

        [Fact]
        public async Task BulkDelete()
        {
            // arrange
            var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA" },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB" },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC" },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD" }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            await db.TestItems.BulkDelete(new Guid[] { initialItems[1].Id, initialItems[3].Id });

            // assert
            var testItems = await db.TestItems.ToArray();
            var expectedItems = new TestItem[] { initialItems[0], initialItems[2] };

            Assert.Equal(2, testItems.Length);

            foreach (var expectedItem in expectedItems)
            {
                Assert.Equal(expectedItem.Name, testItems.First(i => i.Id == expectedItem.Id).Name);
                Assert.Equal(expectedItem.Name, testItems.First(i => i.Id == expectedItem.Id).Name);
            }
        }

        [Fact]
        public async Task BulkGet()
        {
            // arrange
            var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA" },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB" },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC" },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD" }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.BulkGet(new Guid[] { initialItems[0].Id, initialItems[3].Id });

            // assert
            Assert.NotNull(testItems);
            Assert.Equal(2, testItems.Count());
            Assert.Equal("AA", testItems![0]!.Name);
            Assert.Equal("DD", testItems![1]!.Name);
        }

        [Fact]
        public async Task BulkPut()
        {
            // arrange
            var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA" },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB" },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC" },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD" }
            };

            // act
            var lastkey = await db.TestItems.BulkPut(initialItems);

            // assert
            var testItems = await db.TestItems.BulkGet(new Guid[] { initialItems[0].Id, initialItems[1].Id, initialItems[2].Id, initialItems[3].Id });

            Assert.Equal(initialItems[3].Id, lastkey);
            Assert.NotNull(testItems);
            Assert.Equal(4, testItems.Count());
            Assert.Equal("AA", testItems![0]!.Name);
            Assert.Equal("BB", testItems![1]!.Name);
            Assert.Equal("CC", testItems![2]!.Name);
            Assert.Equal("DD", testItems![3]!.Name);
        }

        [Fact]
        public async Task BulkPutReturnAllkeys()
        {
            // arrange
            var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA" },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB" },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC" },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD" }
            };

            // act
            var keys = await db.TestItems.BulkPutReturnAllKeys(initialItems);

            // assert
            var testItems = await db.TestItems.BulkGet(new Guid[] { initialItems[0].Id, initialItems[1].Id, initialItems[2].Id, initialItems[3].Id });

            Assert.Equal(initialItems[0].Id, keys[0]);
            Assert.Equal(initialItems[1].Id, keys[1]);
            Assert.Equal(initialItems[2].Id, keys[2]);
            Assert.Equal(initialItems[3].Id, keys[3]);

            Assert.NotNull(testItems);
            Assert.Equal(4, testItems.Length);
            Assert.Equal("AA", testItems![0]!.Name);
            Assert.Equal("BB", testItems![1]!.Name);
            Assert.Equal("CC", testItems![2]!.Name);
            Assert.Equal("DD", testItems![3]!.Name);
        }

        [Fact]
        public async Task BulkPutHiddenKey()
        {
            // arrange
            var db = CreateDb();

            var initialItems = new TestItemHiddenKey[]
            {
                new TestItemHiddenKey() { Name = "AA" },
                new TestItemHiddenKey() { Name = "BB" },
                new TestItemHiddenKey() { Name = "CC" },
                new TestItemHiddenKey() { Name = "DD" }
            };

            var keys = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            // act
            await db.TestItemsHiddenKey.BulkPut(initialItems, keys);

            // assert
            var testItems = await db.TestItemsHiddenKey.BulkGet(keys);

            Assert.NotNull(testItems);
            Assert.Equal("AA", testItems![0]!.Name);
            Assert.Equal("BB", testItems![1]!.Name);
            Assert.Equal("CC", testItems![2]!.Name);
            Assert.Equal("DD", testItems![3]!.Name);
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

        [Fact]
        public async Task Delete()
        {
            // arrange
            var db = CreateDb();

            var initialItem = new TestItem() { Id = Guid.NewGuid(), Name = "AA" };
            await db.TestItems.Put(initialItem);

            // act
            await db.TestItems.Delete(initialItem.Id);

            // assert
            Assert.True(await db.TestItems.Get(initialItem.Id) == null);
        }

        [Fact]
        public async Task Get()
        {
            // arrange
            var db = CreateDb();

            var initialItem = new TestItem() { Id = Guid.NewGuid(), Name = "AA" };
            await db.TestItems.Put(initialItem);

            // act
            var testItem = await db.TestItems.Get(initialItem.Id);

            // assert
            Assert.NotNull(testItem);
            Assert.Equal("AA", testItem!.Name);
        }

        [Fact]
        public async Task OrderBy()
        {
            // arrange
            var db = CreateDb();

            var initialItems = new TestItem[]{
                new TestItem() { Id = Guid.NewGuid(), Name = "C" },
                new TestItem() { Id = Guid.NewGuid(), Name = "A" },
                new TestItem() { Id = Guid.NewGuid(), Name = "B" }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.OrderBy(nameof(TestItem.Name)).ToArray();

            // assert
            Assert.Equal(3, testItems.Length);
            Assert.Equal(new string[] {"A", "B", "C" }, testItems.Select(t => t.Name).ToArray());
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

            Assert.Equal(key, testItem.Id);
            Assert.NotNull(checkItem);
            Assert.Equal("BB", checkItem!.Name);
        }

        [Fact]
        public async Task PutHiddenKey()
        {
            // arrange
            var db = CreateDb();

            // act
            var testItem = new TestItemHiddenKey() { Name = "BB" };
            var key = Guid.NewGuid();
            await db.TestItemsHiddenKey.Put(testItem, key);

            // assert
            var checkItem = await db.TestItemsHiddenKey.Get(key);

            Assert.NotNull(checkItem);
            Assert.Equal("BB", checkItem!.Name);
        }

        [Fact]
        public async Task Reverse()
        {
            // arrange
            var db = CreateDb();

            var initialItems = new TestItem[]{
                new TestItem() { Id = Guid.NewGuid(), Name = "C" },
                new TestItem() { Id = Guid.NewGuid(), Name = "A" },
                new TestItem() { Id = Guid.NewGuid(), Name = "B" }
            };

            var exceptedNames = initialItems.OrderByDescending(i => i.Id).Select(i => i.Name).ToArray();

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Reverse().ToArray();

            // assert
            Assert.Equal(3, testItems.Length);
            Assert.Equal(exceptedNames, testItems.Select(t => t.Name).ToArray());
        }

        [Fact]
        public async Task Update()
        {
            // arrange
            var db = CreateDb();

            var initialItem = new TestItem() { Id = Guid.NewGuid(), Name = "BB" };
            await db.TestItems.Put(initialItem);

            // act
            var updatedRecord = await db.TestItems.Update(initialItem.Id, new Dictionary<string, object> { { nameof(TestItem.Name), "CC" } });

            // assert
            var checkItem = await db.TestItems.Get(initialItem.Id);

            Assert.Equal(1, updatedRecord);
            Assert.NotNull(checkItem);
            Assert.Equal("CC", checkItem!.Name);
        }

        [Fact]
        public async Task Where()
        {
            // arrange
            var db = CreateDb();

            var initialItems = new TestItem[]
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
            Assert.Equal(initialItems[3].Id, testItems?.First().Id);
        }

        private MyDb CreateDb()
        {
            var moduleFactory = new ModuleWrapperFactory(_nodeJSService, "../../../DexieWrapper/wwwroot");
            return new MyDb(moduleFactory);
        }
    }
}
