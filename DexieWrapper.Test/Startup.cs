using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using Nosthy.Blazor.DexieWrapper.Test.ModuleWrappers;
using Nosthy.Blazor.DexieWrapper.Extensions;

namespace DexieWrapper.Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNodeJS();
            services.AddDexieWrapper(p => new CommonJsModuleFactory(p.GetRequiredService<INodeJSService>(), "../../../DexieWrapper/wwwroot"));
        }
    }
}
