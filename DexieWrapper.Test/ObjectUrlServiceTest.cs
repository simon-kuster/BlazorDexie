using Jering.Javascript.NodeJS;
using Nosthy.Blazor.DexieWrapper.ObjUrl;
using System.Threading.Tasks;
using Xunit;

namespace Nosthy.Blazor.DexieWrapper.Test
{
    public class ObjectUrlServiceTest : IAsyncLifetime
    {
        private ObjectUrlService _objectUrlService;

        public ObjectUrlServiceTest(ObjectUrlService objectUrlService) 
        {
            _objectUrlService = objectUrlService;
        }

        [Fact]
        public async Task Create()
        {
            // arrange
            var initalData = new byte[] { 52, 124, 65, 144 };
            var initalMimetype = "application/test";

            // act
            var objectUrl = await _objectUrlService.Create(initalData, mimeType: initalMimetype);

            // assert
            var data = await _objectUrlService.FetchData(objectUrl);
            Assert.Equal(initalData, data);
        }


        [Fact]
        public async Task FetchData()
        {
            // arrange
            var initalData = new byte[] { 52, 124, 142, 144 };
            var initalMimetype = "application/test";
            var objectUrl = await _objectUrlService.Create(initalData, mimeType: initalMimetype);

            // act
            var data = await _objectUrlService.FetchData(objectUrl);

            // assert

            Assert.Equal(initalData, data);
        }

        [Fact]
        public async Task Revoke()
        {
            // arrange
            var initalData = new byte[] { 52, 124, 142, 148 };
            var initalMimetype = "application/test";
            var objectUrl = await _objectUrlService.Create(initalData, mimeType: initalMimetype);

            // act
            await _objectUrlService.Revoke(objectUrl);

            // assert
            await Assert.ThrowsAsync<InvocationException>(async () => await _objectUrlService.FetchData(objectUrl));
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _objectUrlService.DisposeAsync().ConfigureAwait(false);
        }
    }
}
