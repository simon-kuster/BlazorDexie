using Nosthy.Blazor.DexieWrapper.Test.TestItems;
using System.Threading.Tasks;
using System;
using Xunit;
using Nosthy.Blazor.DexieWrapper.Test.Database;
using Nosthy.Blazor.DexieWrapper.Database;
using Nosthy.Blazor.DexieWrapper.JsModule;

namespace Nosthy.Blazor.DexieWrapper.Test
{
    public class DbTest
    {
        private IModuleFactory _moduleFactory;

        public DbTest(IModuleFactory moduleFactory)
        {
            _moduleFactory = moduleFactory;
        }

        [Fact]
        public async Task Delete()
        {
            // arrange
            await using var db = CreateDb();
            await db.TestItems.Put(new TestItem());

            // act
            await db.Delete();

            // assert
            var exists = await new Dexie(_moduleFactory).Exits(db.DatabaseName);
            Assert.False(exists);
        }

        private MyDb CreateDb()
        {
            var databaseId = Guid.NewGuid().ToString();
           return new MyDb(_moduleFactory, databaseId);
        }
    }
}