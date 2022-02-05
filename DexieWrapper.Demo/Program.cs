using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Nosthy.Blazor.DexieWrapper.Demo;
using Nosthy.Blazor.DexieWrapper.Demo.Database;
using Nosthy.Blazor.DexieWrapper.Demo.Persons;
using Nosthy.Blazor.DexieWrapper.JsModule;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IJsModuleFactory>(sp => new JsObjectReferenceWrapperFactory(sp.GetRequiredService<IJSRuntime>(), "./_content/Nosthy.Blazor.DexieWrapper"));
builder.Services.AddScoped<MyDb>();

builder.Services.AddScoped<PersonRepository>();

await builder.Build().RunAsync();


