using BlazorDexie.Database;
using BlazorDexie.JsModule;
using BlazorDexie.ObjUrl;
using BlazorDexie.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorDexie.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBlazorDexie(this IServiceCollection services, string userModuleBasePath = "", bool camelCaseStoreNames = true)
        {
            services.AddBlazorDexie(p => new EsModuleFactory(p.GetRequiredService<IJSRuntime>(), userModuleBasePath), camelCaseStoreNames);
        }

        public static void AddBlazorDexie(this IServiceCollection services, Func<IServiceProvider, IModuleFactory> moduleFactoryFactory, bool camelCaseStoreNames = true)
        {
            services.AddScoped(p => moduleFactoryFactory(p));
            services.AddScoped<ObjectUrlService>();
            services.AddScoped<Dexie>();
            services.AddTransient(p => new BlazorDexieOptions(p.GetRequiredService<IModuleFactory>(), p.GetRequiredService<ILoggerFactory>()) { CamelCaseStoreNames = camelCaseStoreNames });
        }
    }
}
