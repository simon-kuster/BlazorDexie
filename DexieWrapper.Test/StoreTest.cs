using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Nosthy.Blazor.DexieWrapper.Test.Database;
using Nosthy.Blazor.DexieWrapper.Test.TestItems;
using Nosthy.Blazor.DexieWrapper.JsModule;

namespace Nosthy.Blazor.DexieWrapper.Test
{
    public class StoreTest
    {
        private IModuleFactory _moduleFactory;

        public StoreTest(IModuleFactory moduleFactory)
        {
            _moduleFactory = moduleFactory;
        }

        [Fact]
        public async Task Add()
        {
            // arrange
            await using var db = CreateDb();

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
            await using var db = CreateDb();

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
            await using var db = CreateDb();

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
            await using var db = CreateDb();

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
            await using var db = CreateDb();

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
            await using var db = CreateDb();

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
            await using var db = CreateDb();

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
            await using var db = CreateDb();

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
            await using var db = CreateDb();

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
            await using var db = CreateDb();

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
            await using var db = CreateDb();

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
            await using var db = CreateDb();

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
            await using var db = CreateDb();

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
            await using var db = CreateDb();

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
        public async Task OrderByCompoundIndex()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItemWithCompoundIndex[]{
                new TestItemWithCompoundIndex() { Firstname = "B", Secondname = "A" },
                new TestItemWithCompoundIndex() { Firstname = "A", Secondname = "A" },
                new TestItemWithCompoundIndex() { Firstname = "A", Secondname = "B" }
            };

            await db.TestItemsWithCompoundIndex.BulkPut(initialItems);


            // act
            var testItems = await db.TestItemsWithCompoundIndex.OrderBy(db.TestItemsWithCompoundIndex.Indices[1]).ToArray();

            // assert
            Assert.Equal(3, testItems.Length);
            Assert.Equal(new int?[] { 2, 3, 1 }, testItems.Select(t => t.Id).ToArray());
        }

        [Fact]
        public async Task Put()
        {
            // arrange
            await using var db = CreateDb();

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
            await using var db = CreateDb();

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
        public async Task Update()
        {
            // arrange
            await using var db = CreateDb();

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
        public async Task UpdateHiddenKey()
        {
            // arrange
            await using var db = CreateDb();

            var key = Guid.NewGuid();
            var initialItem = new TestItemHiddenKey() { Name = "BB" };
            await db.TestItemsHiddenKey.Put(initialItem, key);

            // act
            var updatedRecord = await db.TestItemsHiddenKey.Update(key, new Dictionary<string, object> { { nameof(TestItem.Name), "CC" } });

            // assert
            var checkItem = await db.TestItemsHiddenKey.Get(key);

            Assert.Equal(1, updatedRecord);
            Assert.NotNull(checkItem);
            Assert.Equal("CC", checkItem!.Name);
        }

        [Fact]
        public async Task Where()
        {
            // arrange
            await using var db = CreateDb();

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

        [Fact]
        public async Task IndicesAndPrimaryKey()
        {
            // arrange
            await using var db = CreateDb();

            // assert
            Assert.Equal("Id", db.TestItems.PrimaryKey);
            Assert.Equal(new []{ "Id", "Year", "Name" }, db.TestItems.Indices);
        }

        [Fact]
        public async Task HasIndex()
        {
            // arrange
            await using var db = CreateDb();

            // assert
            Assert.True(db.TestItems.HasIndex("Year"));
            Assert.False(db.TestItems.HasIndex("Month"));
        }

        private MyDb CreateDb()
        {
            var databaseId = Guid.NewGuid().ToString();
            return new MyDb(_moduleFactory, databaseId);
        }
    }
}
