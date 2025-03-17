using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using System.Security.Claims;

namespace BlazorWebAppWithSimpleAuthentication;

public class CustomAuthenticationHandler : IAuthenticationHandler
{
  public const string SchemeName = "CustomAuthenticationScheme";
  private const string UserNameHeaderName = "X-Claim-UserName";

  private HttpContext? _httpContext;

  public CustomAuthenticationHandler()
  {
  }

  public Task<AuthenticateResult> AuthenticateAsync()
  {   
    if (this._httpContext is null)
    {
      return Task.FromResult(AuthenticateResult.Fail("No HttpContext"));
    }
    if (!this._httpContext.Request.Headers.TryGetValue(UserNameHeaderName, out var userName) || (userName.Count == 0))
    {
      return Task.FromResult(AuthenticateResult.Fail("No user name found in the request headers."));
    }
    return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(CreateClaimsPrincipal(userName.ToString()), SchemeName)));
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
