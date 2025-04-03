using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Diagnostics;

namespace BlazorWebAppWithSimpleAuthentication.Components;

public abstract class AppComponentBase : ComponentBase
{
  private static long _s_instanceIdSeed;
  private static long _s_counter;
  private readonly long _instanceId;

  protected AppComponentBase()
  {
    this._instanceId = Interlocked.Increment(ref _s_instanceIdSeed);
    Interlocked.Increment(ref _s_counter);
    Debug.WriteLine($"=============> {this.GetType().Name} #{this._instanceId} created, total {_s_counter}.");
  }

  ~AppComponentBase()
  {
    Interlocked.Decrement(ref _s_counter);
    Debug.WriteLine($"=============> {this.GetType().Name}#{this._instanceId} destroyed, total {_s_counter}");
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
