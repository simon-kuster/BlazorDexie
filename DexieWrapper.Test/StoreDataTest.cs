using Jering.Javascript.NodeJS;
using System.Threading.Tasks;
using Xunit;
using Nosthy.Blazor.DexieWrapper.Test.ModuleWrappers;
using Nosthy.Blazor.DexieWrapper.ObjUrl;

namespace Nosthy.Blazor.DexieWrapper.Test
{
    public class StoreDataTest : IAsyncLifetime
    {
        private CommonJsModuleFactory _moduleFactory;
        private ObjectUrlService _objectUrlService;

        public StoreDataTest(INodeJSService nodeJSService)
        {
            _moduleFactory = new CommonJsModuleFactory(nodeJSService, "../../../DexieWrapper/wwwroot");
            _objectUrlService = new ObjectUrlService(_moduleFactory);
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
