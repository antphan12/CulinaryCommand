# Senior Design Website

Production site:
- [https://sdmay26-44.sd.ece.iastate.edu](https://sdmay26-44.sd.ece.iastate.edu)

Testing application:
- [http://culinary-command.com/](http://culinary-command.com/)
- `http://3.20.198.36/`

## Playwright Tests

The Playwright UI test project lives in [PlaywrightTests](/Users/wyatthunter/projects/sdmay26-44/PlaywrightTests).

Current admin coverage summary:
- Dashboard access, Cognito login redirect, and navigation
- Manage users create, invite verification, edit, cancel, and validation flows
- Recipe and ingredient create/edit/filter/delete flows
- Inventory management lifecycle, search, and delete-cancel flows

Full coverage summary:
- See [TEST_COVERAGE.md](/Users/wyatthunter/projects/sdmay26-44/TEST_COVERAGE.md)

## Playwright Requirements

To run the Playwright xUnit suite locally, you need:
- .NET 9 SDK
- The app running locally at `http://localhost:5256`
- A valid admin login available through the local app auth flow
- `PLAYWRIGHT_ADMIN_EMAIL` and `PLAYWRIGHT_ADMIN_PASSWORD` environment variables set
- Playwright browser binaries installed for the .NET test project

Admin tests use stored auth state:
- `authState.admin.json`
- This file is created automatically by the admin auth fixture if it does not already exist

Important behavior:
- If you change Razor pages or Blazor components that the tests exercise, restart the local app before rerunning tests so Playwright hits the updated build
- The most reliable place to run test commands from is the `PlaywrightTests` directory

## Playwright Setup

From the repo root:

```bash
cd PlaywrightTests
dotnet build
pwsh bin/Debug/net9.0/playwright.ps1 install
```

Set credentials before running admin-authenticated tests:

```bash
export PLAYWRIGHT_ADMIN_EMAIL="your-admin-email"
export PLAYWRIGHT_ADMIN_PASSWORD="your-admin-password"
```

## Useful Test Commands

Run the full manage-users coverage:

```bash
cd PlaywrightTests
HEADED=0 dotnet test --filter "FullyQualifiedName~ManageUsers"
```

Run the full recipe coverage:

```bash
cd PlaywrightTests
HEADED=0 dotnet test --filter "FullyQualifiedName~RecipeTests"
```

Run the inventory management coverage:

```bash
cd PlaywrightTests
HEADED=0 dotnet test --filter "FullyQualifiedName~InventoryManagementTests"
```

Run the recipe lifecycle test only:

```bash
cd PlaywrightTests
HEADED=0 dotnet test --filter "FullyQualifiedName~Admin_Recipe_Lifecycle_Create_Edit_ProduceValidation_Delete"
```

Run headed for debugging:

```bash
cd PlaywrightTests
PWDEBUG=1 HEADED=1 dotnet test --filter "FullyQualifiedName~ManageUsers"
```

## Current Admin Test Files

- [PlaywrightTests/Tests/ActualTests/AdminTests/DashboardTests.cs](/Users/wyatthunter/projects/sdmay26-44/PlaywrightTests/Tests/ActualTests/AdminTests/DashboardTests.cs)
- [PlaywrightTests/Tests/ActualTests/AdminTests/ManageUsersTests.cs](/Users/wyatthunter/projects/sdmay26-44/PlaywrightTests/Tests/ActualTests/AdminTests/ManageUsersTests.cs)
- [PlaywrightTests/Tests/ActualTests/AdminTests/ManageUsers_EditTests.cs](/Users/wyatthunter/projects/sdmay26-44/PlaywrightTests/Tests/ActualTests/AdminTests/ManageUsers_EditTests.cs)
- [PlaywrightTests/Tests/ActualTests/AdminTests/RecipeTests.cs](/Users/wyatthunter/projects/sdmay26-44/PlaywrightTests/Tests/ActualTests/AdminTests/RecipeTests.cs)
- [PlaywrightTests/Tests/ActualTests/AdminTests/InventoryManagementTests.cs](/Users/wyatthunter/projects/sdmay26-44/PlaywrightTests/Tests/ActualTests/AdminTests/InventoryManagementTests.cs)
