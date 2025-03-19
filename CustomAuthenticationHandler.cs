using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using System.Security.Claims;

namespace BlazorWebAppWithSimpleAuthentication;

public class CustomAuthenticationHandler : IAuthenticationHandler
{
  public const string SchemeName = "CustomAuthenticationScheme";
  private const string UserNameHeaderName = "X-Claim-UserName";
  private const string CookieName = "CustomAuthCookie";

  private static long _s_counter;
  private HttpContext? _httpContext;

  public CustomAuthenticationHandler()
  {
    Console.WriteLine($"CustomAuthenticationHandler #{Interlocked.Increment(ref _s_counter)} created.");
  }


  public Task<AuthenticateResult> AuthenticateAsync()
  {
    if (this._httpContext is null)
    {
      return Task.FromResult(AuthenticateResult.Fail("No HttpContext"));
    }

    if (this._httpContext.Request.Cookies.TryGetValue(CookieName, out var userNameFromCookie))
    {
      Console.WriteLine($"==================> User name from COOKIE: {userNameFromCookie}");
      return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(CreateClaimsPrincipal(userNameFromCookie), SchemeName)));
    }

    if (!this._httpContext.Request.Headers.TryGetValue(UserNameHeaderName, out var userNameFromHeader) || (userNameFromHeader.Count == 0))
    {
      return Task.FromResult(AuthenticateResult.NoResult());
    }

    Console.WriteLine($"==================> User name from HEADER: {userNameFromHeader}");
    this._httpContext.Response.Cookies.Append(CookieName, userNameFromHeader.ToString());
    return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(CreateClaimsPrincipal(userNameFromHeader.ToString()), SchemeName)));
  }


  public Task ChallengeAsync(AuthenticationProperties? properties)
  {
    return Task.CompletedTask;
  }


  public Task ForbidAsync(AuthenticationProperties? properties)
  {
    return Task.CompletedTask;
  }


  public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
  {
    Console.WriteLine($"Initializing {scheme.Name} authentication scheme for {context.Request.GetDisplayUrl()}, method {context.Request.Method}.");
    this._httpContext = context;
    return Task.CompletedTask;
  }


  private ClaimsPrincipal CreateClaimsPrincipal(string userName = "DEFAULT")
  {
    var claims = new[] { new Claim(ClaimTypes.Name, userName) };
    var identity = new ClaimsIdentity(claims, SchemeName);
    return new ClaimsPrincipal(identity);

  }
}
