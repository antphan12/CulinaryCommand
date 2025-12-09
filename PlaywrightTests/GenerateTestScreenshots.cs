using Microsoft.Playwright;

namespace PlaywrightTests;

public class GenerateTestScreenshots
{
    [Fact]
    public async Task home_page_screenshot()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        var page = await browser.NewPageAsync();

        await page.GotoAsync("http://localhost:5000");

        var artifactsDir = Path.Combine("CulinaryCommandApp", "playwright-artifacts", "screenshots");
        Directory.CreateDirectory(artifactsDir);

        var screenshotPath = Path.Combine(artifactsDir, "home-page.png");
        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = screenshotPath,
            FullPage = true
        });
    }
}
