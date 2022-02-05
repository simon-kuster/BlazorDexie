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
        public async Task IsEqual()
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
            var testItems = await db.TestItems.Where(nameof(TestItem.Year)).IsEqual(2012).ToArray();

            // assert
            TestItem[] expectedItems = new TestItem[] { initialItems[1], initialItems[2] };

            Assert.Equal(2, testItems.Length);
            foreach (var exptectedItem in expectedItems)
            {
                Assert.Equal(exptectedItem.Id, testItems.First(i => i.Name == exptectedItem.Name).Id);
            }

        }

        private MyDb CreateDb()
        {
            var moduleFactory = new ModuleWrapperFactory(_nodeJSService, "../../../DexieWrapper/wwwroot");
            return new MyDb(moduleFactory);
        }
    }
}
