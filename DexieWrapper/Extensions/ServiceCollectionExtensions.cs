using Microsoft.Extensions.DependencyInjection;
using Nosthy.Blazor.DexieWrapper.Database;
using Nosthy.Blazor.DexieWrapper.JsModule;
using Nosthy.Blazor.DexieWrapper.ObjUrl;

namespace Nosthy.Blazor.DexieWrapper.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDexieWrapper(this IServiceCollection services)
        {
            services.AddScoped<IModuleFactory, EsModuleFactory>();
            services.AddScoped<ObjectUrlService>();
            services.AddScoped<Dexie>();
        }
    }
}
