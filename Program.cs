using BlazorWebAppWithSimpleAuthentication.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using System.Diagnostics;
using System.Security.Claims;

namespace BlazorWebAppWithSimpleAuthentication;

public class Program
{
  private static int _s_id = 0;

  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    // Add our custom authentication scheme and handler for request headers-based authentication..
    builder.Services.AddAuthentication(options =>
    {
      options.AddScheme<CustomAuthenticationHandler>(
        name: CustomAuthenticationHandler.SchemeName, 
        displayName: CustomAuthenticationHandler.SchemeName);
    });      

    // Add services to the container.
    IRazorComponentsBuilder razorComponentsBuilder = builder.Services.AddRazorComponents();
    razorComponentsBuilder.AddInteractiveServerComponents();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
      app.UseExceptionHandler("/Error");
      // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
      app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseAntiforgery();

    app.UseAuthentication();
    app.MapStaticAssets();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.Run();
  }
}
