using CulinaryCommand.Data;
using CulinaryCommand.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CulinaryCommand.Services
{
    public class TaskAssignmentService : ITaskAssignmentService
    {
        private readonly AppDbContext _db;

        public TaskAssignmentService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Tasks>> GetByLocationAsync(int locationId)
        {
            return await _db.Tasks
                .Where(t => t.LocationId == locationId)
                .Include(t => t.User)

                // Load recipe + ingredients + units + steps so UI can show details
                .Include(t => t.Recipe)
                    .ThenInclude(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                .Include(t => t.Recipe)
                    .ThenInclude(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Unit)
                .Include(t => t.Recipe)
                    .ThenInclude(r => r.Steps)

                // If you still need the task-level Ingredient, keep this:
                .Include(t => t.Ingredient)

                .OrderByDescending(t => t.DueDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Tasks> CreateAsync(Tasks task)
        {
            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();
            return task;
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return;

            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task BumpDueDateAsync(int id, int days)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return;

            task.DueDate = task.DueDate.AddDays(days);
            task.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return;

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Tasks>> GetForUserAsync(int userId, int? locationId = null)
        {
            var query = _db.Tasks
                .Where(t => t.UserId == userId);

            if (locationId.HasValue)
            {
                query = query.Where(t => t.LocationId == locationId.Value);
            }

            return await query
                .Include(t => t.User)

                // Same eager-loading here so My Tasks page gets everything it needs
                .Include(t => t.Recipe)
                    .ThenInclude(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                .Include(t => t.Recipe)
                    .ThenInclude(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Unit)
                .Include(t => t.Recipe)
                    .ThenInclude(r => r.Steps)
                .Include(t => t.Ingredient)

                .OrderBy(t => t.DueDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
