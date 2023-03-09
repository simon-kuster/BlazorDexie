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
            var db = new MyDb(ModuleFactory);

            // act
            var initalData = new byte[] { 213, 23, 55, 234, 11 };
            var key = Guid.NewGuid();
            await db.BlobData.AddBlob(initalData, key, "application/octet-stream");

            // assert
            var data = await db.BlobData.GetBlob(key);
        }

        private async Task PutBlob()
        {
            var db = new MyDb(ModuleFactory);

            // act
            var initalData = new byte[] { 213, 23, 55, 234, 22 };
            var key = Guid.NewGuid();
            await db.BlobData.PutBlob(initalData, key, "application/octet-stream");

            // assert
            var data = await db.BlobData.GetBlob(key);
        }

        private async Task AddObjectUrl()
        {
            var db = new MyDb(ModuleFactory);

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
            var db = new MyDb(ModuleFactory);

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
            var db = new MyDb(ModuleFactory);
            await db.Delete();
        }
    }
}
