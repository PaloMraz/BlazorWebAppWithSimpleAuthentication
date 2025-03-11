using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWebAppWithSimpleAuthentication.Components;

public static class AppComponentExtensions
{
  public static async Task InitializeUserNameAsync(this Task<AuthenticationState>? authenticationState, Action<string> userNameSetter)
  {
    if (authenticationState is not null)
    {
      AuthenticationState state = await authenticationState;
      userNameSetter(state.User.Identity?.Name ?? "Anonymous");
    }
    else
    {
      userNameSetter("Unknown");
    }
  }
}
