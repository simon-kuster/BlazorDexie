using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using BlazorDexie.Extensions;
using BlazorDexie.Test.ModuleWrappers;

namespace BlazorDexie.Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNodeJS();
            services.AddDexieWrapper(p => new CommonJsModuleFactory(p.GetRequiredService<INodeJSService>(), "../../../BlazorDexie/wwwroot"));
        }
    }
}
