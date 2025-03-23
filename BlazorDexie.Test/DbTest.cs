using System.Threading.Tasks;
using System;
using Xunit;
using BlazorDexie.Test.Database;
using BlazorDexie.Test.TestItems;
using BlazorDexie.Database;
using BlazorDexie.Options;

namespace BlazorDexie.Test
{
    public class DbTest
    {
        private BlazorDexieOptions _blazorDexieOptions;

        public DbTest(BlazorDexieOptions blazorDexieOptions)
        {
            _blazorDexieOptions = blazorDexieOptions;
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
            var exists = await new Dexie(_blazorDexieOptions).Exits(db.DatabaseName, TestContext.Current.CancellationToken);
            Assert.False(exists);
        }

        private MyDb CreateDb()
        {
            var databaseId = Guid.NewGuid().ToString();
            return new MyDb(_blazorDexieOptions, databaseId);
        }
    }
}