using BlazorWebAppWithSimpleAuthentication.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
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
    IRazorComponentsBuilder razorComponentsBuilder = builder.Services.AddRazorComponents();
    razorComponentsBuilder.AddInteractiveServerComponents();

    // Set of claims that are initialized in the inline middleware below from request headers and
    // used to create the user's identity through the CustomAuthenticationStateProvider
    List<Claim> claimsFromRequestHeaders = new();

    // Add the CustomAuthenticationStateProvider to the service collection with a delegate that closes
    // over the claimsFromRequestHeaders initialized by the inline middleware below.
    builder.Services.AddScoped<AuthenticationStateProvider>(implementationFactory: (serviceProvider) => 
    {
      return new CustomAuthenticationStateProvider(claimsFromRequestHeaders);
    });

    var app = builder.Build();

    // Inline middleware that initializes the claimsFromRequestHeaders collection from the first request.
    app.Use(async (context, next) =>
    {
      // Do not initialize more than once.
      if (claimsFromRequestHeaders.Count > 0)
      {
        await next();
        return;
      }

      // This would be initialized from request headers...
      if (context.Request.Headers.TryGetValue("X-Claim-UserId", out var userId))
      {
        Console.WriteLine($"X-Claim-UserId: {userId}");
        claimsFromRequestHeaders.Add(new Claim(ClaimTypes.NameIdentifier, userId!));
      }
      if (context.Request.Headers.TryGetValue("X-Claim-UserName", out var userName))
      {
        Console.WriteLine($"X-Claim-UserName: {userName}");
        claimsFromRequestHeaders.Add(new Claim(ClaimTypes.Name, userName!));
      }
      await next();
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


/// <summary>
/// Creates an <see cref="AuthenticationState"/> with claims provider in constructor.
/// </summary>
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
  private readonly IReadOnlyCollection<Claim> _claims;

  public CustomAuthenticationStateProvider(IReadOnlyCollection<Claim> claims)
  {
    this._claims = claims;
  }


  public override Task<AuthenticationState> GetAuthenticationStateAsync()
  {
    var identity = new ClaimsIdentity(this._claims, "Custom Authentication");
    var user = new ClaimsPrincipal(identity);
    return Task.FromResult(new AuthenticationState(user));
  }
}
