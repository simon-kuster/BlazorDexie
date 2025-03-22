using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using BlazorDexie.Extensions;
using BlazorDexie.Test.ModuleWrappers;
using BlazorDexie.JsModule;
using Microsoft.Extensions.Logging;
using BlazorDexie.Options;

namespace BlazorDexie.Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNodeJS();
            services.AddDexieWrapper(p => new CommonJsModuleFactory(p.GetRequiredService<INodeJSService>(), 
                "../../../BlazorDexie/wwwroot", 
                "../../../BlazorDexie.Test/wwwroot/"));

            services.AddTransient(p => new BlazorDexieOptions(p.GetRequiredService<IModuleFactory>(), p.GetRequiredService<ILoggerFactory>()) { CamelCaseStoreNames = true });
        }
    }
}
