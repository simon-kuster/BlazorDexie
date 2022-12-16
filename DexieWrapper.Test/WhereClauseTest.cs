using Jering.Javascript.NodeJS;
using Nosthy.Blazor.DexieWrapper.Test.Database;
using Nosthy.Blazor.DexieWrapper.Test.ModuleWrappers;
using Nosthy.Blazor.DexieWrapper.Test.TestItems;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nosthy.Blazor.DexieWrapper.Test
{
    public class WhereClauseTest
    {
        private INodeJSService _nodeJSService;

        public WhereClauseTest(INodeJSService nodeJSService)
        {
            _nodeJSService = nodeJSService;
        }

        [Fact]
        public async Task Above()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2023 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2022 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2020 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2011 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Year)).Above(2020).ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[0], initialItems[1] };

            Assert.Equal(2, testItems.Length);
            foreach (var item in expectedItems)
            {
                Assert.Equal(item.Id, testItems.First(i => i.Year == item.Year).Id);
            }
        }

        [Fact]
        public async Task AboveOrEqual()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2023 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2022 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2020 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2011 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Year)).AboveOrEqual(2020).ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[0], initialItems[1], initialItems[2] };

            Assert.Equal(3, testItems.Length);
            foreach (var item in expectedItems)
            {
                Assert.Equal(item.Id, testItems.First(i => i.Year == item.Year).Id);
            }
        }

        [Fact]
        public async Task AnyOf()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2023 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2022 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2020 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2011 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Year)).AnyOf(2022, 2020, 2023).ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[0], initialItems[1], initialItems[2] };

            foreach (var item in expectedItems)
            {
                Assert.Equal(item.Id, testItems.First(i => i.Name == item.Name).Id);
            }
        }

        [Fact]
        public async Task AnyOfIngoreCase()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2023 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2022 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2020 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2011 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Name)).AnyOfIgnoreCase("bB", "aA", "cC").ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[0], initialItems[1], initialItems[2] };

            foreach (var item in expectedItems)
            {
                Assert.Equal(item.Id, testItems.First(i => i.Name == item.Name).Id);
            }
        }

        [Fact]
        public async Task Below()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2023 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2022 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2020 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2011 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Year)).Below(2022).ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[2], initialItems[3] };

            Assert.Equal(2, testItems.Length);
            foreach (var item in expectedItems)
            {
                Assert.Equal(item.Id, testItems.First(i => i.Year == item.Year).Id);
            }
        }

        [Fact]
        public async Task BelowOrEqual()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2023 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2022 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2020 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2011 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Year)).BelowOrEqual(2022).ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[1], initialItems[2], initialItems[3] };

            Assert.Equal(3, testItems.Length);
            foreach (var item in expectedItems)
            {
                Assert.Equal(item.Id, testItems.First(i => i.Year == item.Year).Id);
            }
        }

        [Fact]
        public async Task Between()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2023 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2022 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2020 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2011 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Year)).Between(2020, 2023).ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[1], initialItems[2] };

            Assert.Equal(2, testItems.Length);
            foreach (var item in expectedItems)
            {
                Assert.Equal(item.Id, testItems.First(i => i.Year == item.Year).Id);
            }
        }

        [Fact]
        public async Task IsEqual()
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
            var testItems = await db.TestItems.Where(nameof(TestItem.Year)).IsEqual(2012).ToArray();

            // assert
            var expectedItems = new TestItem[] { initialItems[1], initialItems[2] };

            Assert.Equal(2, testItems.Length);
            foreach (var exptectedItem in expectedItems)
            {
                Assert.Equal(exptectedItem.Id, testItems.First(i => i.Name == exptectedItem.Name).Id);
            }

        }

        [Fact]
        public async Task EqualIgnoreCase()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2010 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2012 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2013 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2015 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Name)).EqualIgnoreCase("BB").ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[1], initialItems[2] };

            Assert.Equal(2, testItems.Length);
            foreach (var exptectedItem in expectedItems)
            {
                Assert.Equal(exptectedItem.Id, testItems.First(i => i.Year == exptectedItem.Year).Id);
            }

        }

        [Fact]
        public async Task InAnyRange()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2010 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2011 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2012 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2013 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2014 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2015 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2016 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Year)).InAnyRange(new object[][] { new object[] { 2010, 2011 }, new object[] { 2014, 2016 } }).ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[0], initialItems[4], initialItems[5] };

            Assert.Equal(3, testItems.Length);
            foreach (var exptectedItem in expectedItems)
            {
                Assert.Equal(exptectedItem.Id, testItems.First(i => i.Year == exptectedItem.Year).Id);
            }

        }

        [Fact]
        public async Task NoneOf()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2010 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2012 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2013 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2015 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Year)).NoneOf(2010, 2012).ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[2], initialItems[3] };

            Assert.Equal(2, testItems.Length);
            foreach (var exptectedItem in expectedItems)
            {
                Assert.Equal(exptectedItem.Id, testItems.First(i => i.Name == exptectedItem.Name).Id);
            }

        }

        [Fact]
        public async Task NotEqual()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2010 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2012 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2013 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2015 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Year)).NotEqual(2012).ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[0], initialItems[2], initialItems[3] };

            Assert.Equal(3, testItems.Length);
            foreach (var exptectedItem in expectedItems)
            {
                Assert.Equal(exptectedItem.Id, testItems.First(i => i.Name == exptectedItem.Name).Id);
            }

        }

        [Fact]
        public async Task StartsWith()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2010 },
                new TestItem() { Id = Guid.NewGuid(), Name = "AB", Year = 2012 },
                new TestItem() { Id = Guid.NewGuid(), Name = "AC", Year = 2013 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2015 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Name)).StartsWith("A").ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[0], initialItems[1], initialItems[2] };

            Assert.Equal(3, testItems.Length);
            foreach (var exptectedItem in expectedItems)
            {
                Assert.Equal(exptectedItem.Id, testItems.First(i => i.Year == exptectedItem.Year).Id);
            }
        }

        [Fact]
        public async Task StartsWithAnyOf()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2010 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2012 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2013 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2015 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Name)).StartsWithAnyOf("A", "B").ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[0], initialItems[1] };

            Assert.Equal(2, testItems.Length);
            foreach (var exptectedItem in expectedItems)
            {
                Assert.Equal(exptectedItem.Id, testItems.First(i => i.Year == exptectedItem.Year).Id);
            }
        }

        [Fact]
        public async Task StartsWithIgnoreCase()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2010 },
                new TestItem() { Id = Guid.NewGuid(), Name = "AB", Year = 2012 },
                new TestItem() { Id = Guid.NewGuid(), Name = "AC", Year = 2013 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2015 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Name)).StartsWithIgnoreCase("a").ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[0], initialItems[1], initialItems[2] };

            Assert.Equal(3, testItems.Length);
            foreach (var exptectedItem in expectedItems)
            {
                Assert.Equal(exptectedItem.Id, testItems.First(i => i.Year == exptectedItem.Year).Id);
            }
        }

        [Fact]
        public async Task StartsWithAnyOfIgnoreCase()
        {
            // arrange
            await using var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2010 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2012 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2013 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2015 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Name)).StartsWithAnyOfIgnoreCase("a", "b").ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[0], initialItems[1] };

            Assert.Equal(2, testItems.Length);
            foreach (var exptectedItem in expectedItems)
            {
                Assert.Equal(exptectedItem.Id, testItems.First(i => i.Year == exptectedItem.Year).Id);
            }
        }

        private MyDb CreateDb()
        {
            var databaseId = Guid.NewGuid().ToString();
            var moduleFactory = new CommonJsModuleFactory(_nodeJSService, "../../../DexieWrapper/wwwroot");
            return new MyDb(moduleFactory, databaseId);
        }
    }
}
