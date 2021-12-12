using DexieWrapper.Demo;
using DexieWrapper.JsModule;
using DexieWrapper.Persons;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IJsModuleFactory>(sp => new JsObjectReferenceWrapperFactory(sp.GetRequiredService<IJSRuntime>(), "./_content/DexieWrapper"));
builder.Services.AddScoped<PersonRepository>();

await builder.Build().RunAsync();


