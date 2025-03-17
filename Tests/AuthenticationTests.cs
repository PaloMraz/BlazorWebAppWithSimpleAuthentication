using Microsoft.Playwright;
using System.Diagnostics;

namespace Tests;

public class AuthenticationTests
{
  [Fact]
  public async Task HasUserName()
  {
    using var playwright = await Playwright.CreateAsync();
    await using IBrowser browser = await playwright.Chromium.LaunchAsync();

    List<Task> invokeTasks = new();
    for (int i = 0; i < 5; i++)
    {
      string userName = $"user-{i}";
      invokeTasks.Add(ExecuteUserNameCheckAsync(browser, userName));
    }
    await Task.WhenAll(invokeTasks);
  }


  [Fact]
  public async Task VerifyEndToEndInteraction()
  {
    string hostProcessExePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\bin\publish\BlazorWebAppWithSimpleAuthentication.exe"));
    using var hostProcess = Process.Start(hostProcessExePath, "--urls https://localhost:7175");

    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
      Headless = false
    });

    const string UserName = "Fero Mrvenica";
    var context = await browser.NewContextAsync(new BrowserNewContextOptions()
    {
      ExtraHTTPHeaders = new Dictionary<string, string>()
      {
        ["X-Claim-UserName"] = UserName
      }
    });

    var page = await context.NewPageAsync();
    await page.GotoAsync("https://localhost:7175/");
    await page.GetByRole(AriaRole.Link, new() { Name = $"Go to Secure Page for Superuser Role" }).ClickAsync();
    ILocator message = page.GetByText("You are not authorized to view this page.");
    Assert.NotNull(message);

    hostProcess.Kill();
  }

  private static async Task ExecuteUserNameCheckAsync(IBrowser browser, string userName)
  {
    await using IBrowserContext context = await browser.NewContextAsync(new BrowserNewContextOptions()
    {
      ExtraHTTPHeaders = new Dictionary<string, string>()
      {
        ["X-Claim-UserName"] = userName
      }
    });
    var page = await context.NewPageAsync();
    await page.GotoAsync("https://localhost:7175");

    var userNameLocator = page.GetByTestId("user-name");
    await Assertions.Expect(userNameLocator).ToHaveTextAsync(userName);
  }
}
