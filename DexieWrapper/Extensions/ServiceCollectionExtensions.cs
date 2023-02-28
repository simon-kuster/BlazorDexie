using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Nosthy.Blazor.DexieWrapper.Database;
using Nosthy.Blazor.DexieWrapper.JsModule;
using Nosthy.Blazor.DexieWrapper.ObjUrl;

namespace Nosthy.Blazor.DexieWrapper.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDexieWrapper(this IServiceCollection services)
        {
            AddDexieWrapper(services, p => new EsModuleFactory(p.GetRequiredService<IJSRuntime>()));
        }

        public static void AddDexieWrapper(this IServiceCollection services, Func<IServiceProvider, IModuleFactory> moduleFactoryFactory)
        {
            services.AddScoped<IModuleFactory>(p => moduleFactoryFactory(p));
            services.AddScoped<ObjectUrlService>();
            services.AddScoped<Dexie>();
        }
    }
}
