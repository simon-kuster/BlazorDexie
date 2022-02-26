using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using Nosthy.Blazor.DexieWrapper.Test.Database;
using Nosthy.Blazor.DexieWrapper.Test.ModuleWrappers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nosthy.Blazor.DexieWrapper.Test
{
    public class WhereClauseTest
    {
        private INodeJSService _nodeJSService;

        public WhereClauseTest()
        {
            var services = new ServiceCollection();
            services.AddNodeJS();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            _nodeJSService = serviceProvider.GetRequiredService<INodeJSService>();
        }

        [Fact]
        public async Task Above()
        {
            // arrange
            var db = CreateDb();

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
            var db = CreateDb();

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
            var db = CreateDb();

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
            var db = CreateDb();

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
            var db = CreateDb();

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
            var db = CreateDb();

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
            var db = CreateDb();

            var initialItems = new TestItem[]
            {
                new TestItem() { Id = Guid.NewGuid(), Name = "AA", Year = 2023 },
                new TestItem() { Id = Guid.NewGuid(), Name = "BB", Year = 2022 },
                new TestItem() { Id = Guid.NewGuid(), Name = "CC", Year = 2020 },
                new TestItem() { Id = Guid.NewGuid(), Name = "DD", Year = 2011 }
            };

            await db.TestItems.BulkPut(initialItems);

            // act
            var testItems = await db.TestItems.Where(nameof(TestItem.Year)).Betweent(2020, 2023).ToArray();

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
            var db = CreateDb();

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
            var db = CreateDb();

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
            var db = CreateDb();

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
            var db = CreateDb();

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
            var db = CreateDb();

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
            var db = CreateDb();

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
            var db = CreateDb();

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
            var db = CreateDb();

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
            var moduleFactory = new ModuleWrapperFactory(_nodeJSService, "../../../DexieWrapper/wwwroot");
            return new MyDb(moduleFactory);
        }
    }
}
