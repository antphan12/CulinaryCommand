namespace CulinaryCommand.Inventory.Services.Interfaces
{
    public interface IIngredientService
    {
        // Retrieves all ingredients; returns a list of Ingredient from the data store.
        Task<List<Ingredient>> GetAllAsync(CancellationToken cancellationToken = default);

        // Retrieves a single ingredient by its id; returns null if not found.
        Task<Ingredient?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Creates a new ingredient in the data store.
        Task<Ingredient> CreateAsync(Ingredient ingredient, CancellationToken cancellationToken = default);

        // Updates an existing ingredient in the data store.
        Task UpdateAsync(Ingredient ingredient, CancellationToken cancellationToken = default);

        // Deletes an ingredient by id from the data store.
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}