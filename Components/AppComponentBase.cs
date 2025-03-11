using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWebAppWithSimpleAuthentication.Components;

public abstract class AppComponentBase : ComponentBase
{
  protected AppComponentBase()
  {
  }

  protected async override Task OnInitializedAsync()
  {
    await base.OnInitializedAsync();
    await this.AuthenticationState.InitializeUserNameAsync((userName) => this.UserName = userName);
  }

  public string UserName { get; private set; } = "";


  [CascadingParameter]
  public Task<AuthenticationState>? AuthenticationState { get; set; }
}
