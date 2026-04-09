using Microsoft.Playwright;
using PlaywrightTests.Tests.Base;

namespace PlaywrightTests.Tests.ActualTests.AdminTests;

public class ManageUsers_EditTests : AuthenticatedTestBase
{
    private const string UsersUrl = "http://localhost:5256/users";
    private const int Timeout = 20000;

    [Fact]
    public async Task Admin_Edit_User_Name_And_Save()
    {
        var unique = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var email = $"edituser{unique}@test.com";

        await CreateUser_FromUsersPage("PW", "EditTest", email);

        var userItem = await WaitForUserRow(email);

        await userItem.Locator(".accordion-button").ClickAsync();

        await userItem.Locator("button:has-text('Edit')").Last.ClickAsync();

        await Page.WaitForURLAsync("**/users/edit/**");

        await Page.Locator("input.form-control").First.FillAsync("Updated Name");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await Page.WaitForTimeoutAsync(1500);
        await Page.GotoAsync(UsersUrl);

        await Expect(Page.Locator(".accordion-item").Filter(new() { HasText = "Updated Name" }))
            .ToBeVisibleAsync();

        await DeleteUser(email);
    }

    [Fact]
    public async Task Admin_Edit_User_Cancel()
    {
        var unique = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var email = $"canceluser{unique}@test.com";

        await CreateUser_FromUsersPage("PW", "CancelTest", email);

        var userItem = await WaitForUserRow(email);

        await userItem.Locator(".accordion-button").ClickAsync();

        await userItem.Locator("button:has-text('Edit')").Last.ClickAsync();

        await Page.WaitForURLAsync("**/users/edit/**");

        await Page.Locator("input.form-control").First.FillAsync("SHOULD NOT SAVE");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).ClickAsync();

        await Page.WaitForTimeoutAsync(1000);
        await Page.GotoAsync(UsersUrl);

        await Expect(Page.Locator(".accordion-item").Filter(new() { HasText = email }))
            .ToBeVisibleAsync();

        await DeleteUser(email);
    }

    [Fact]
    public async Task Admin_Create_User_Validation_Email_Required()
    {
        await Page.GotoAsync("http://localhost:5256/users/create");

        var inputs = Page.Locator("input.form-control");

        await inputs.Nth(0).FillAsync("Test");
        await inputs.Nth(1).FillAsync("User");
        await inputs.Nth(2).FillAsync("");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Send Invite" }).ClickAsync();

        await Expect(Page.GetByText("Email is required.")).ToBeVisibleAsync();
    }

    // 🔥 USE SAME FLOW AS WORKING TEST
    private async Task CreateUser_FromUsersPage(string first, string last, string email)
    {
        await Page.GotoAsync(UsersUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Add User" }).ClickAsync();

        var inputs = Page.Locator("input.form-control");

        await inputs.Nth(0).FillAsync(first);
        await inputs.Nth(1).FillAsync(last);
        await inputs.Nth(2).FillAsync(email);

        await Page.Locator("select.form-select")
            .SelectOptionAsync(new SelectOptionValue { Label = "Manager" });

        await Page.GetByRole(AriaRole.Button, new() { Name = "Send Invite" }).ClickAsync();

        await Page.WaitForTimeoutAsync(1500);
        await Page.GotoAsync(UsersUrl);
    }

    private async Task<ILocator> WaitForUserRow(string email)
    {
        var userItem = Page.Locator(".accordion-item").Filter(new() { HasText = email }).First;
        var deadline = DateTime.UtcNow.AddMilliseconds(Timeout);

        while (DateTime.UtcNow < deadline)
        {
            if (await userItem.CountAsync() > 0 && await userItem.IsVisibleAsync())
                return userItem;

            await Page.WaitForTimeoutAsync(1000);
            await Page.ReloadAsync();
        }

        throw new Exception($"User {email} not found in UI.");
    }

    private async Task DeleteUser(string email)
    {
        var userItem = await WaitForUserRow(email);

        await userItem.Locator(".accordion-button").ClickAsync();

        userItem.Locator("button:has-text('Delete')").Last.ClickAsync();

        await Page.WaitForTimeoutAsync(1000);
        await Page.ReloadAsync();
    }
}