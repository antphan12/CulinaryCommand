# Playwright Test Coverage

This file summarizes the current Playwright coverage in this repository.

## Admin Dashboard

Source: [PlaywrightTests/Tests/ActualTests/AdminTests/DashboardTests.cs](/Users/wyatthunter/projects/sdmay26-44/PlaywrightTests/Tests/ActualTests/AdminTests/DashboardTests.cs)

Current coverage:
- Authenticated admin is redirected away from `/login` to `/dashboard` or `/tasks`
- Admin can load `/dashboard`
- Admin lands on `/dashboard`
- Admin can see the `Add User` button on `/users`
- Admin can navigate from dashboard to `/users`
- Admin can access admin-only users pages without being redirected to login

## Manage Users

Source: [PlaywrightTests/Tests/ActualTests/AdminTests/ManageUsersTests.cs](/Users/wyatthunter/projects/sdmay26-44/PlaywrightTests/Tests/ActualTests/AdminTests/ManageUsersTests.cs)

Current coverage:
- Create a user from `/users`
- Verify the created user appears in the users list
- Expand the user row and capture the invite link
- Open the invite link in a fresh browser context
- Verify the account setup page loads
- Delete the created user
- Verify the created user is removed

Source: [PlaywrightTests/Tests/ActualTests/AdminTests/ManageUsers_EditTests.cs](/Users/wyatthunter/projects/sdmay26-44/PlaywrightTests/Tests/ActualTests/AdminTests/ManageUsers_EditTests.cs)

Current coverage:
- Edit a user's name and save the change
- Edit a user and cancel without saving
- Validate create-user flow when email is missing
- Validate create-user flow when email format is invalid

## Recipes And Ingredients

Source: [PlaywrightTests/Tests/ActualTests/AdminTests/RecipeTests.cs](/Users/wyatthunter/projects/sdmay26-44/PlaywrightTests/Tests/ActualTests/AdminTests/RecipeTests.cs)

Current coverage:
- Create an ingredient from `/inventory-catalog`
- Create a recipe from `/recipes/create`
- Verify the recipe appears in `/recipes`
- Edit the recipe title
- Open recipe view and open the Produce modal
- Verify `0` servings is rejected in the Produce flow
- Delete the created recipe
- Clean up created recipe and ingredient test data
- Create a sub-recipe
- Filter and verify the sub-recipe on the `Sub-Recipes / Prep Items` tab

## Shared Test Pattern

Shared setup:
- Authenticated admin tests inherit from [PlaywrightTests/Tests/Base/AuthenticatedTestBase.cs](/Users/wyatthunter/projects/sdmay26-44/PlaywrightTests/Tests/Base/AuthenticatedTestBase.cs)
- Admin tests reuse stored auth state from `authState.admin.json`
- Admin auth state is created by [PlaywrightTests/Tests/Setup/AuthHelper.cs](/Users/wyatthunter/projects/sdmay26-44/PlaywrightTests/Tests/Setup/AuthHelper.cs) and [PlaywrightTests/Tests/Setup/AdminAuthFixture.cs](/Users/wyatthunter/projects/sdmay26-44/PlaywrightTests/Tests/Setup/AdminAuthFixture.cs)

Suite conventions:
- Use resilient selectors instead of brittle label lookups where the markup is not fully associated
- Prefer explicit page-ready helpers and reload-based waits for async UI state
- Use `try/finally` cleanup for generated test data
- Dismiss lingering modals or overlays before cleanup actions when needed
