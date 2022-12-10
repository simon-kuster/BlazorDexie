using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;

namespace DexieWrapper.Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNodeJS();
        }
    }
}
