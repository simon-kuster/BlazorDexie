using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Nosthy.Blazor.DexieWrapper.Blob;
using Nosthy.Blazor.DexieWrapper.Demo.Database;
using Nosthy.Blazor.DexieWrapper.Demo.Persons;
using Nosthy.Blazor.DexieWrapper.Demo.TestsItems;
using Nosthy.Blazor.DexieWrapper.ObjUrl;
using Nosthy.Blazor.DexieWrapper.JsModule;

namespace Nosthy.Blazor.DexieWrapper.Demo.Pages
{
    public partial class Index
    {
        private Person? _person;
        private string? _imgSrc; 

        [Inject] public PersonRepository PersonRepository { get; set; } = null!;
        [Inject] public IModuleFactory ModuleFactory { get; set; } = null!;

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

        private async Task CreateBild(InputFileChangeEventArgs e)
        {
            using var stream = e.File.OpenReadStream(100000000);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            using var objectUrlService = new ObjectUrlService(ModuleFactory);
            var blobData = await CreateBlobData(memoryStream.ToArray(), objectUrlService);

            var image = new ImageDataItem
            {
                Id = Guid.NewGuid(),
                Data = blobData
            };

            var db = new MyDb(ModuleFactory);
            await db.ImageDataItems.Put(image);
        }

        private async Task LoadBild()
        {
            var db = new MyDb(ModuleFactory);
            var firstImage = (await db.ImageDataItems.ToArray(blobDataFormat: BlobDataFormat.ObjectUrl)).FirstOrDefault();

            if (firstImage != null)
            {
                _imgSrc = firstImage.Data?.ObjectUrl;
            }
        }

        private async Task<BlobData> CreateBlobData(byte[] data, ObjectUrlService objectUrlService)
        {
            var objectUrl = await objectUrlService.Create(data);
            return new BlobData(objectUrl: objectUrl);
        }

        private async Task<byte[]> GetData(BlobData blobData, ObjectUrlService objectUrlService)
        {

            if (blobData.ByteArray != null)
            {
                return blobData.ByteArray;
            }

            if (blobData.ObjectUrl != null)
            {
                var data = await objectUrlService.FetchDataNode(blobData.ObjectUrl);
                await objectUrlService.Revoke(blobData.ObjectUrl);
                return data;
            }

            return new byte[0];
        }
    }
}
