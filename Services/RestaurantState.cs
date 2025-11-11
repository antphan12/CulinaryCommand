using CulinaryCommand.Models;

namespace CulinaryCommand.Services
{
    public class RestaurantState
    {
        public RestaurantModel? CurrentRestaurant { get; private set; }

        public event Action? OnChange;

        public void SetRestaurant(RestaurantModel restaurant)
        {
            CurrentRestaurant = restaurant;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
