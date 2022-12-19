using Nosthy.Blazor.DexieWrapper.Database;
using Nosthy.Blazor.DexieWrapper.JsModule;
using Nosthy.Blazor.DexieWrapper.Test.Database;
using Nosthy.Blazor.DexieWrapper.Test.TestItems;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Nosthy.Blazor.DexieWrapper.Test
{
    public class DexieTest
    {
        private IModuleFactory _moduleFactory;
        private Dexie _dexie;

        public DexieTest(IModuleFactory moduleFactory, Dexie dexie)
        {
            _moduleFactory = moduleFactory;
            _dexie = dexie;
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
            var exists = await _dexie.Exits(db.DefaultDatabaseName);
            Assert.False(exists);
        }

        [Fact]
        public async Task Exists()
        {
            // arrange
            string databaseId = Guid.NewGuid().ToString();

            // not exits
            var exists = await _dexie.Exits(databaseId);
            Assert.False(exists);

            // create db
            await using var db = CreateDb(databaseId);
            await db.TestItems.Put(new TestItem());

            // exists
            exists = await _dexie.Exits(db.DefaultDatabaseName);
            Assert.True(exists);
        }

        private MyDb CreateDb(string? databaseId = null)
        {
            return new MyDb(_moduleFactory, databaseId ?? Guid.NewGuid().ToString());
        }
    }
}
