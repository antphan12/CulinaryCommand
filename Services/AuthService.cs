using CulinaryCommand.Data.Entities;
using Microsoft.JSInterop;
using System.Text.Json;

namespace CulinaryCommand.Services
{
    public class AuthService
    {
        private readonly IJSRuntime _js;
        private bool _hydrated;

        public AuthService(IJSRuntime js) { _js = js; }

        public bool IsSignedIn { get; private set; }
        public string? CurrentUser { get; private set; }
        public string? UserRole { get; private set; }
        public string? Company { get; private set; }
        public string? CompanyCode { get; private set; }
        public string? Location { get; private set; }

        public int? CompanyId { get; private set; }
        public List<int>? LocationIds { get; private set; }
        public int? ActiveLocationId { get; private set; }

        public event Action? OnAuthStateChanged;
        private void Raise() => OnAuthStateChanged?.Invoke();

        public List<string>? Locations { get; set; }

        public void NotifyStateChanged() => OnAuthStateChanged?.Invoke();


        public async Task InitializeFromJsAsync()
        {
            if (_hydrated) return;
            try
            {
                CurrentUser = await _js.InvokeAsync<string?>("localStorage.getItem", "cc_user");
                UserRole = await _js.InvokeAsync<string?>("localStorage.getItem", "cc_role");
                Company = await _js.InvokeAsync<string?>("localStorage.getItem", "cc_company");
                CompanyCode = await _js.InvokeAsync<string?>("localStorage.getItem", "cc_companyCode");

                var companyIdStr = await _js.InvokeAsync<string?>("localStorage.getItem", "cc_companyId");
                if (int.TryParse(companyIdStr, out int companyId))
                {
                    CompanyId = companyId;
                }
                else
                {
                    CompanyId = null;
                }

                var locationIdsJson = await _js.InvokeAsync<string?>("localStorage.getItem", "cc_locationIds");
                if (!string.IsNullOrEmpty(locationIdsJson))
                {
                    try
                    {
                        LocationIds = JsonSerializer.Deserialize<List<int>>(locationIdsJson);
                    }
                    catch
                    {
                        LocationIds = null;
                    }
                }
                else
                {
                    LocationIds = null;
                }

                var activeLocationIdStr = await _js.InvokeAsync<string?>("localStorage.getItem", "cc_activeLocationId");
                if (int.TryParse(activeLocationIdStr, out int activeLocId))
                {
                    ActiveLocationId = activeLocId;
                }
                else
                {
                    ActiveLocationId = null;
                }

                Location = await _js.InvokeAsync<string?>("localStorage.getItem", "cc_location");

                IsSignedIn = !string.IsNullOrEmpty(CurrentUser);
            }
            catch
            {
                // ignore; treat as signed out
                IsSignedIn = false;
            }
            finally
            {
                _hydrated = true;
                Raise();
            }
        }

        public async Task SignInAsync(User user)
        {
            CurrentUser = user.Email;
            UserRole = user.Role;
            Company = user.Company?.Name;
            CompanyCode = user.Company?.CompanyCode;
            CompanyId = user.CompanyId;
            Locations = user.UserLocations
                ?.Select(ul => ul.Location.Name)
                .ToList();

            LocationIds = user.UserLocations
                ?.Select(ul => ul.LocationId)
                .ToList();

            ActiveLocationId = LocationIds?.FirstOrDefault();

            Location = user.UserLocations
                .Select(ul => ul.Location.Name)
                .FirstOrDefault();

            IsSignedIn = true;

            await _js.InvokeVoidAsync("localStorage.setItem", "cc_user", CurrentUser ?? "");
            await _js.InvokeVoidAsync("localStorage.setItem", "cc_role", UserRole ?? "");
            await _js.InvokeVoidAsync("localStorage.setItem", "cc_company", Company ?? "");
            await _js.InvokeVoidAsync("localStorage.setItem", "cc_companyCode", CompanyCode ?? "");
            await _js.InvokeVoidAsync("localStorage.setItem", "cc_companyId", CompanyId?.ToString() ?? "");
            await _js.InvokeVoidAsync("localStorage.setItem", "cc_locationIds", JsonSerializer.Serialize(LocationIds ?? new List<int>()));
            await _js.InvokeVoidAsync("localStorage.setItem", "cc_activeLocationId", ActiveLocationId?.ToString() ?? "");
            await _js.InvokeVoidAsync("localStorage.setItem", "cc_location", Location ?? "");
            Raise();
        }

        public async Task LogoutAsync()
        {
            CurrentUser = null;
            UserRole = Company = CompanyCode = Location = null;
            CompanyId = null;
            LocationIds = null;
            ActiveLocationId = null;
            IsSignedIn = false;

            await _js.InvokeVoidAsync("localStorage.removeItem", "cc_user");
            await _js.InvokeVoidAsync("localStorage.removeItem", "cc_role");
            await _js.InvokeVoidAsync("localStorage.removeItem", "cc_company");
            await _js.InvokeVoidAsync("localStorage.removeItem", "cc_companyCode");
            await _js.InvokeVoidAsync("localStorage.removeItem", "cc_companyId");
            await _js.InvokeVoidAsync("localStorage.removeItem", "cc_locationIds");
            await _js.InvokeVoidAsync("localStorage.removeItem", "cc_activeLocationId");
            await _js.InvokeVoidAsync("localStorage.removeItem", "cc_location");
            Raise();
        }

        public async Task<string?> GetUser()
        {
            return await _js.InvokeAsync<string?>("localStorage.getItem", "cc_user");
        }

        public async Task<bool> IsUserSignedIn()
        {
            string? User = await _js.InvokeAsync<string?>("localStorage.getItem", "cc_user");
            return User != null ? true : false;
        }

        public async Task UpdateLocation(string newLoc)
        {
            this.Location = newLoc;
            await _js.InvokeVoidAsync("localStorage.setItem", "cc_location", this.Location ?? "");
        }

        public async Task UpdateActiveLocation(int newLocId)
        {
            this.ActiveLocationId = newLocId;
            await _js.InvokeVoidAsync("localStorage.setItem", "cc_activeLocationId", newLocId.ToString());
        }

    }
}