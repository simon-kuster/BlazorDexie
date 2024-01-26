using BlazorDexie.Demo.Database;
using BlazorDexie.Demo.Persons;
using BlazorDexie.JsModule;
using BlazorDexie.ObjUrl;
using Microsoft.AspNetCore.Components;

namespace BlazorDexie.Demo.Pages
{
    public partial class Index
    {
        private Person? _person;

        [Inject] public PersonRepository PersonRepository { get; set; } = null!;
        [Inject] public IModuleFactory ModuleFactory { get; set; } = null!;
        [Inject] public ObjectUrlService ObjectUrlService { get; set; } = null!;

        private async Task CreatePersonClicked()
        {
            _person = new Person()
            {
                Id = Guid.NewGuid(),
                FirstName = "Dario",
                SecondName = "Kuster"
            };

            await PersonRepository.CreateOrUpdate(_person);
        }

        private async Task ReadPersonClicked()
        {
            _person = (await PersonRepository.GetAll()).FirstOrDefault();
        }

        private async Task UpdatePersonClicked()
        {
            if (_person == null)
            {
                throw new Exception("No person exists. Create person before update");
            }

            _person.Birthday = new DateTime(2008, 2, 1);

            await PersonRepository.CreateOrUpdate(_person);
        }

        private async Task DeletePersonClicked()
        {
            if (_person == null)
            {
                throw new Exception("No person exists. Create person before update");
            }

            await PersonRepository.Delete(_person);
            _person = null;
        }

        private async Task AddBlob()
        {
            await using var db = new MyDb(ModuleFactory);

            // act
            var initalData = new byte[] { 213, 23, 55, 234, 11 };
            var key = Guid.NewGuid();
            await db.BlobData.AddBlob(initalData, key, "application/octet-stream");

            // assert
            var data = await db.BlobData.GetBlob(key);
        }

        private async Task BulkAddBlob()
        {
            await using var db = new MyDb(ModuleFactory);

            // act
            var initialDatas = new byte[][] { [213, 23, 55, 234, 54], [23, 23, 44], [11, 22, 33] };
            var keys = Enumerable.Range(0, initialDatas.Length).Select(_ => Guid.NewGuid()).ToArray();

#pragma warning disable CS0618 // Type or member is obsolete
            await db.BlobData.BulkAddBlob(initialDatas, keys, "application/octet-stream");
#pragma warning restore CS0618 // Type or member is obsolete

            // assert
            var data = await db.BlobData.GetBlob(keys[0]);
        }

        private async Task PutBlob()
        {
            await using var db = new MyDb(ModuleFactory);

            // act
            var initalData = new byte[] { 213, 23, 55, 234, 22 };
            var key = Guid.NewGuid();
            await db.BlobData.PutBlob(initalData, key, "application/octet-stream");

            // assert
            var data = await db.BlobData.GetBlob(key);
        }

        private async Task AddObjectUrl()
        {
            await using var db = new MyDb(ModuleFactory);

            // act
            var initalData = new byte[] { 213, 23, 55, 234, 33 };
            var initalObjectUrl = await ObjectUrlService.Create(initalData, "application/octet-stream");

            var key = Guid.NewGuid();
            await db.BlobData.AddObjectUrl(initalObjectUrl, key);

            // assert
            var objectUrl = await db.BlobData.GetObjectUrl(key);
            var data = await ObjectUrlService.FetchData(objectUrl);
            await ObjectUrlService.Revoke(objectUrl);
        }

        private async Task PutObjectUrl()
        {
            await using var db = new MyDb(ModuleFactory);

            // act
            var initalData = new byte[] { 213, 23, 55, 234, 44 };
            var initalObjectUrl = await ObjectUrlService.Create(initalData, "application/octet-stream");
            var key = Guid.NewGuid();
            await db.BlobData.PutObjectUrl(initalObjectUrl, key);

            // assert
            var data = await db.BlobData.GetBlob(key);
        }

        private async Task DeleteDb()
        {
            await using var db = new MyDb(ModuleFactory);
            await db.Delete();
        }

        private async Task TransactionCompleted()
        {
            // arrange
            await using var db = new MyDb(ModuleFactory);
            var key = await db.Persons.Put(new Person() { FirstName = "Hans", Birthday = new DateTime(1970, 1, 1) });

            // act
            try
            {
                await db.Transaction("rw", [nameof(MyDb.Persons)], 60000, async () =>
                {
                    var person = await db.Persons.Get(key) ?? throw new InvalidOperationException();
                    person.Birthday = new DateTime(1971, 1, 1);
                    await db.Persons.Put(person);
                });
            }
            catch (Exception e)
            {
                // handle error
            }

            var check = await db.Persons.Get(key) ?? throw new InvalidOperationException();
        }

        private async Task TransactionFailed()
        {
            // arrange
            await using var db = new MyDb(ModuleFactory);
            var key = await db.Persons.Put(new Person() { FirstName = "Hans", Birthday = new DateTime(1970, 1, 1) });

            // act
            try
            {
                await db.Transaction("rw", [nameof(MyDb.Persons)], 60000, async () =>
                {
                    var person = await db.Persons.Get(key) ?? throw new InvalidOperationException();
                    person.Birthday = new DateTime(1971, 1, 1);
                    await db.Persons.Put(person);
                    await db.BlobData.Get(key);
                });
            }
            catch(Exception e)
            {
                // handle error
            }

            var check = await db.Persons.Get(key) ?? throw new InvalidOperationException();
        }
    }
}
