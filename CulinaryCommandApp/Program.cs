using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

using CulinaryCommand.Data;
using CulinaryCommand.Services;
using CulinaryCommand.Inventory.Services;
using CulinaryCommand.PurchaseOrder.Services;
using CulinaryCommand.Inventory;
using CulinaryCommand.Inventory.Services.Interfaces;
using CulinaryCommand.Components;
using CulinaryCommandApp.AIDashboard.Services.Reporting;
using Google.GenAI;
using System;

var builder = WebApplication.CreateBuilder(args);

//
// =====================
// UI
// =====================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//
// =====================
// Cognito Authentication (MUST be before Build)
// =====================
// ===== Cognito Auth (OIDC) =====
builder.Services
  .AddAuthentication(options =>
  {
      options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
  })
  .AddCookie()
  .AddOpenIdConnect(options =>
  {
      var userPoolId = "us-east-2_SULe0c9vr";
      var region = "us-east-2";

      options.Authority = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}";
      options.MetadataAddress = $"{options.Authority}/.well-known/openid-configuration";

      options.ClientId = "55joip0viah9qtj7dndhvma2gt";
      options.ClientSecret = Environment.GetEnvironmentVariable("COGNITO_CLIENT_SECRET"); // don’t hardcode

      options.ResponseType = OpenIdConnectResponseType.Code;
      options.SaveTokens = true;

      options.CallbackPath = "/signin-oidc";
      options.SignedOutCallbackPath = "/signout-callback-oidc";

      options.RequireHttpsMetadata = true; // keep true

      options.Scope.Clear();
      options.Scope.Add("openid");
      options.Scope.Add("email");
      options.Scope.Add("profile");

      options.TokenValidationParameters.NameClaimType = "cognito:username";
      options.TokenValidationParameters.RoleClaimType = "cognito:groups";
  });

builder.Services.AddAuthorization();


//
// =====================
// AI Services
// =====================
builder.Services.AddSingleton<Client>(_ => new Client());
builder.Services.AddScoped<AIReportingService>();

//
// =====================
// Database
// =====================
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(conn))
{
    throw new InvalidOperationException(
        "Missing connection string 'DefaultConnection'");
}

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(
        conn,
        new MySqlServerVersion(new Version(8, 0, 36)),
        mySqlOpts => mySqlOpts.EnableRetryOnFailure()
    )
);

//
// =====================
// Application Services
// =====================
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<UnitService>();
builder.Services.AddScoped<IngredientService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<LocationState>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IInventoryTransactionService, InventoryTransactionService>();
builder.Services.AddScoped<IInventoryManagementService, InventoryManagementService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<ITaskAssignmentService, TaskAssignmentService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddSingleton<EnumService>();

//
// =====================
// Build App
// =====================
var app = builder.Build();

//
// =====================
// Apply EF Migrations
// =====================
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Startup] Migration warning: {ex.Message}");
    }
}

//
// =====================
// Middleware
// =====================
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();
  
//
// =====================
// Routes
// =====================
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/health", () => "OK");

app.MapGet("/login", async (HttpContext ctx) =>
{
    await ctx.ChallengeAsync(
        OpenIdConnectDefaults.AuthenticationScheme,
        new AuthenticationProperties { RedirectUri = "/" }
    );
});

app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme,
        new AuthenticationProperties { RedirectUri = "/" });
});

app.Run();
