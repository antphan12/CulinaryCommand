using PlaywrightTests.Tests.Base;
using PlaywrightTests.Tests.Setup;
using Microsoft.Playwright;

namespace PlaywrightTests.Tests.ActualTests.AdminTests;

[Collection("Admin Auth Collection")]
public class DashboardTests : AuthenticatedTestBase
{
    [Fact]
    public async Task Admin_Login_ShouldRedirectToDashboardOrTasks()
    {
        await Page.GotoAsync("http://localhost:5256/login");

        await Page.WaitForURLAsync(url =>
            url.Contains("/dashboard") || url.Contains("/tasks"),
            new() { Timeout = 15_000 });

        Assert.True(
            Page.Url.Contains("/dashboard") || Page.Url.Contains("/tasks"),
            $"Expected dashboard or tasks, but got: {Page.Url}");
    }

    [Fact]
    public async Task Admin_Dashboard_ShouldLoad()
    {
        await Page.GotoAsync("http://localhost:5256/dashboard");

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.DoesNotContain("/login", Page.Url);
        Assert.DoesNotContain("/account", Page.Url);
    }

    [Fact]
    public async Task Admin_Should_Land_On_Dashboard()
    {
        await Page.GotoAsync("http://localhost:5256/dashboard");

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.Contains("/dashboard", Page.Url);
    }

    [Fact]
    public async Task Admin_Should_See_AddUser_Button()
    {
        await Page.GotoAsync("http://localhost:5256/users");

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var addUserButton = Page.GetByRole(AriaRole.Button, new() { Name = " Add User " });

        await Expect(addUserButton).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Admin_Should_Navigate_To_Users_Page()
    {
        await Page.GotoAsync("http://localhost:5256/dashboard");

        await Page.ClickAsync("text=Users");

        await Page.WaitForURLAsync(url => url.Contains("/users"));

        Assert.Contains("/users", Page.Url);
    }

    [Fact]
    public async Task Admin_Should_Access_Admin_Page()
    {
        await Page.GotoAsync("http://localhost:5256/users");

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.DoesNotContain("/login", Page.Url);
    }
}