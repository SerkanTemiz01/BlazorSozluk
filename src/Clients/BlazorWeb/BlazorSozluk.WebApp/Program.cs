using Blazored.LocalStorage;
using BlazorSozluk.WebApp;
using BlazorSozluk.WebApp.Infrastructure.Services;
using BlazorSozluk.WebApp.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");




builder.Services.AddHttpClient("WebApiClient", client =>
{
    string apiUrl = builder.Configuration["ApiUrl"];    
    client.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("WebApiClient"));

builder.Services.AddTransient<IVoteService, VoteService>();
builder.Services.AddTransient<IFavService, FavService>();
builder.Services.AddTransient<IEntryService, EntryService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IIdentityService, IdentityService>();


//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
