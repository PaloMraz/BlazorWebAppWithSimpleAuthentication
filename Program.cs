using BlazorWebAppWithSimpleAuthentication.Components;
using Microsoft.AspNetCore.Http.Extensions;
using System.Diagnostics;
using System.Security.Claims;

namespace BlazorWebAppWithSimpleAuthentication;

public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    var app = builder.Build();
    app.Use((context, next) =>
    {
      Debug.WriteLine($"======================> {context.Request.GetDisplayUrl()}");
      Claim[] claims =
      [
        new Claim(ClaimTypes.Name, "John Doe"),
        new Claim(ClaimTypes.Email, "john.doe@example.com"),
        new Claim(ClaimTypes.Role, "Administrator")
      ];
      context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomAuth"));
      return next(context);
    });

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
      app.UseExceptionHandler("/Error");
      // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
      app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.Run();
  }
}
