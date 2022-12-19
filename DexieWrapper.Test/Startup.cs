using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using Nosthy.Blazor.DexieWrapper.JsModule;
using Nosthy.Blazor.DexieWrapper.ObjUrl;
using Nosthy.Blazor.DexieWrapper.Test.ModuleWrappers;
using Nosthy.Blazor.DexieWrapper.Database;

namespace DexieWrapper.Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNodeJS();
           
            services.AddTransient<IModuleFactory>(provider =>
              new CommonJsModuleFactory(provider.GetRequiredService<INodeJSService>(), "../../../DexieWrapper/wwwroot"));

            services.AddTransient(provider => new ObjectUrlService(provider.GetRequiredService<IModuleFactory>()));

            services.AddTransient(provider => new Dexie(provider.GetRequiredService<IModuleFactory>()));
        }
    }
}
