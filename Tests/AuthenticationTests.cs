using Microsoft.Playwright;

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
