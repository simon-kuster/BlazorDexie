using BlazorDexie.Database;
using BlazorDexie.JsModule;
using BlazorDexie.ObjUrl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace BlazorDexie.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDexieWrapper(this IServiceCollection services, string userModuleBasePath = "")
        {
            services.AddDexieWrapper(p => new EsModuleFactory(p.GetRequiredService<IJSRuntime>(), userModuleBasePath));
        }

        public static void AddDexieWrapper(this IServiceCollection services, Func<IServiceProvider, IModuleFactory> moduleFactoryFactory)
        {
            services.AddScoped(p => moduleFactoryFactory(p));
            services.AddScoped<ObjectUrlService>();
            services.AddScoped<Dexie>();
        }
    }
}
