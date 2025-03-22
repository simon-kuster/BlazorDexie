using BlazorDexie.Demo;
using BlazorDexie.Demo.Database;
using BlazorDexie.Demo.Logging;
using BlazorDexie.Demo.Persons;
using BlazorDexie.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Logging.ClearProviders(); // Remove default providers
builder.Logging.AddProvider(new SingleLineConsoleLoggerProvider());

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazorDexie();

builder.Services.AddScoped<MyDb>();
builder.Services.AddScoped<PersonRepository>();

await builder.Build().RunAsync();


