using BlazorDexie.JsModule;
using System.Threading.Tasks;
using System;
using Xunit;
using BlazorDexie.Test.V1;
using BlazorDexie.Test.V2;
using BlazorDexie.Test.V3;
using System.Linq;

namespace BlazorDexie.Test
{
    public class VersioningTest
    {
        private IModuleFactory _moduleFactory;

        public VersioningTest(IModuleFactory moduleFactory)
        {
            _moduleFactory = moduleFactory;
        }

        [Fact]
        public async Task Migrate()
        {
            // arrange
            var databaseId = Guid.NewGuid().ToString();
            await using var dbVersion1 = new Db1(_moduleFactory, databaseId);
            await dbVersion1.Friends.Add(new Friend1() { Name = "David", Age = 40, Sex = "Male" }, TestContext.Current.CancellationToken);
            await dbVersion1.Friends.Add(new Friend1() { Name = "Ylva", Age = 39, Sex = "Female" }, TestContext.Current.CancellationToken);

            // act
            await using var dbVersion3 = new Db3(_moduleFactory, databaseId);
            var david3 = (await dbVersion3.Friends.Where(nameof(Friend3.Name)).IsEqual("David").ToList(TestContext.Current.CancellationToken)).First();

            // assert
            var secondsInYear = 365 * 24 * 60 * 60;
            Assert.Equal(DateTime.UtcNow.AddSeconds(-40 * secondsInYear).Date, david3.BirthDate.Date);
            Assert.Equal(1, david3.IsAdult);
        }
    }
}
