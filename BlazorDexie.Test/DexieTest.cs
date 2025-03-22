using BlazorDexie.Database;
using BlazorDexie.JsModule;
using BlazorDexie.Options;
using BlazorDexie.Test.Database;
using BlazorDexie.Test.TestItems;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BlazorDexie.Test
{
    public class DexieTest
    {
        private BlazorDexieOptions _blazorDexieOptions;
        private Dexie _dexie;

        public DexieTest(BlazorDexieOptions blazorDexieOptions, Dexie dexie)
        {
            _blazorDexieOptions = blazorDexieOptions;
            _dexie = dexie;
        }

        [Fact]
        public async Task Delete()
        {
            // arrange
            await using var db = CreateDb();
            await db.TestItems.Put(new TestItem(), TestContext.Current.CancellationToken);

            // act
            await db.Delete(TestContext.Current.CancellationToken);

            // assert
            var exists = await _dexie.Exits(db.DatabaseName, TestContext.Current.CancellationToken);
            Assert.False(exists);
        }

        [Fact]
        public async Task Exists()
        {
            // arrange
            string databaseId = Guid.NewGuid().ToString();

            // not exits
            var exists = await _dexie.Exits(databaseId, TestContext.Current.CancellationToken);
            Assert.False(exists);

            // create db
            await using var db = CreateDb(databaseId);
            await db.TestItems.Put(new TestItem(), TestContext.Current.CancellationToken);

            // exists
            exists = await _dexie.Exits(db.DatabaseName, TestContext.Current.CancellationToken);
            Assert.True(exists);
        }

        private MyDb CreateDb(string? databaseId = null)
        {
            return new MyDb(_blazorDexieOptions, databaseId ?? Guid.NewGuid().ToString());
        }
    }
}
