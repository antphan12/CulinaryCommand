using System.Collections.Concurrent;
using System.Text.Json;
using CulinaryCommand.Data;
using CulinaryCommand.Data.Entities;
using CulinaryCommand.Data.Enums;
using CulinaryCommand.Services;
using CulinaryCommandApp.SmartTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using WorkTaskStatus = CulinaryCommand.Data.Enums.TaskStatus;

namespace CulinaryCommandApp.SmartTask.Services
{
    public sealed class SmartTaskService : ISmartTaskService
    {
        private static readonly ConcurrentDictionary<Guid, SmartTaskPlanPreview> PlanPreviewCache = new();

        private readonly AppDbContext _dbContext;
        private readonly ISmartTaskOrchestratorClient _orchestratorClient;
        private readonly ITaskNotificationService _taskNotificationService;
        private readonly IConfiguration _configuration;

        public SmartTaskService(
            AppDbContext dbContext,
            ISmartTaskOrchestratorClient orchestratorClient,
            ITaskNotificationService taskNotificationService,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _orchestratorClient = orchestratorClient;
            _taskNotificationService = taskNotificationService;
            _configuration = configuration;
        }

        public async Task<SmartTaskPlanPreview> PreviewAsync(
            SmartTaskRequest request, CancellationToken cancellationToken)
        {
            var requestedRecipes = await _dbContext.Recipes
                .AsNoTracking()
                .Where(r => r.LocationId == request.LocationId && request.RecipeIds.Contains(r.RecipeId))
                .Include(r => r.Steps)
                .ToListAsync(cancellationToken);

            var eligibleUsers = await BuildEligibleUserPoolAsync(
                request.LocationId, request.ServiceDate, cancellationToken);

            var planRequestDto = new PlanRequestDto(
                LocationId: request.LocationId,
                ServiceDate: request.ServiceDate,
                Recipes: requestedRecipes.Select(MapRecipeToDto).ToList(),
                EligibleUsers: eligibleUsers,
                Defaults: new PlanDefaultsDto(
                    DefaultPrepBufferMinutes: _configuration.GetValue("SmartTask:DefaultPrepBufferMinutes", 30),
                    DefaultLeadTimeWhenUnknown: _configuration.GetValue("SmartTask:DefaultLeadTimeWhenUnknown", 60)));

            var plan = await _orchestratorClient.RequestPlanAsync(planRequestDto, cancellationToken);

            var preview = new SmartTaskPlanPreview
            {
                OriginalRequest = request,
                Plan = plan
            };
            PlanPreviewCache[preview.PreviewId] = preview;
            return preview;
        }

        public async Task<SmartTaskRun> CommitAsync(Guid planPreviewId, CancellationToken cancellationToken)
        {
            if (!PlanPreviewCache.TryRemove(planPreviewId, out var preview))
                throw new InvalidOperationException("Plan preview expired or not found. Regenerate the plan.");

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            var smartTaskRun = new SmartTaskRun
            {
                LocationId = preview.OriginalRequest.LocationId,
                TriggeredByUserId = preview.OriginalRequest.TriggeredByUserId,
                ServiceDate = preview.OriginalRequest.ServiceDate,
                RecipeIdsJson = JsonSerializer.Serialize(preview.OriginalRequest.RecipeIds),
                CreatedTaskIdsJson = "[]",
                Status = nameof(SmartTaskRunStatus.Planned)
            };
            _dbContext.SmartTaskRuns.Add(smartTaskRun);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var createdTasks = preview.Plan.PlannedTasks
                .Select(plannedTask => BuildTaskFromPlan(plannedTask, smartTaskRun, preview.OriginalRequest))
                .ToList();

            _dbContext.Tasks.AddRange(createdTasks);
            await _dbContext.SaveChangesAsync(cancellationToken);

            smartTaskRun.CreatedTaskIdsJson = JsonSerializer.Serialize(createdTasks.Select(t => t.Id));
            smartTaskRun.Status = nameof(SmartTaskRunStatus.Committed);
            smartTaskRun.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            foreach (var createdTask in createdTasks)
            {
                _taskNotificationService.NotifyTaskAssigned(createdTask);
            }
            return smartTaskRun;
        }

        public async Task RollbackAsync(Guid runId, CancellationToken cancellationToken)
        {
            var smartTaskRun = await _dbContext.SmartTaskRuns
                .FirstOrDefaultAsync(r => r.Id == runId, cancellationToken)
                ?? throw new InvalidOperationException("SmartTask run not found.");

            if (smartTaskRun.Status == nameof(SmartTaskRunStatus.RolledBack))
                return;

            var createdTaskIds = JsonSerializer.Deserialize<List<int>>(smartTaskRun.CreatedTaskIdsJson)
                ?? new List<int>();

            var tasksFromRun = await _dbContext.Tasks
                .Where(t => createdTaskIds.Contains(t.Id))
                .ToListAsync(cancellationToken);

            var anyTaskAlreadyStarted = tasksFromRun.Any(t =>
                t.Status == WorkTaskStatus.InProgress || t.Status == WorkTaskStatus.Completed);

            if (anyTaskAlreadyStarted)
                throw new InvalidOperationException(
                    "One or more tasks are already in progress or completed; rollback is blocked.");

            _dbContext.Tasks.RemoveRange(tasksFromRun);
            smartTaskRun.Status = nameof(SmartTaskRunStatus.RolledBack);
            smartTaskRun.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<List<SmartTaskRun>> RecentRunsAsync(int locationId, int take = 10) =>
            _dbContext.SmartTaskRuns
                .AsNoTracking()
                .Where(r => r.LocationId == locationId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(take)
                .ToListAsync();

        private async Task<List<EligibleUserDto>> BuildEligibleUserPoolAsync(
            int locationId, DateOnly serviceDate, CancellationToken cancellationToken)
        {
            var serviceDayStartUtc = new DateTime(serviceDate, TimeOnly.MinValue, DateTimeKind.Utc);
            var serviceDayEndUtc = serviceDayStartUtc.AddDays(1);

            var eligibleUsersAtLocation = await _dbContext.UserLocations
                .Where(ul => ul.LocationId == locationId
                          && ul.User != null
                          && ul.User.IsActive
                          && ul.User.Role != Roles.Admin)
                .Select(ul => ul.User!)
                .ToListAsync(cancellationToken);

            var openTaskCountByUser = await _dbContext.Tasks
                .Where(t => t.LocationId == locationId
                         && t.UserId != null
                         && t.DueDate >= serviceDayStartUtc
                         && t.DueDate < serviceDayEndUtc)
                .GroupBy(t => t.UserId!.Value)
                .Select(g => new { UserId = g.Key, OpenCount = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.OpenCount, cancellationToken);

            return eligibleUsersAtLocation
                .Select(user => new EligibleUserDto(
                    UserId: user.Id,
                    DisplayName: user.Name ?? user.Email ?? $"User {user.Id}",
                    OpenTaskCountToday: openTaskCountByUser.GetValueOrDefault(user.Id, 0)))
                .ToList();
        }

        private static RecipeInputDto MapRecipeToDto(CulinaryCommandApp.Recipe.Entities.Recipe recipe) =>
            new(
                RecipeId: recipe.RecipeId,
                Title: recipe.Title,
                Category: recipe.Category,
                RecipeType: recipe.RecipeType,
                ServiceWindow: recipe.ServiceWindow?.ToString(),
                ServiceTimeOverride: recipe.ServiceTimeOverride,
                PrepLeadTimeMinutesOverride: recipe.PrepLeadTimeMinutes,
                Steps: recipe.Steps
                    .OrderBy(step => step.StepNumber)
                    .Select(step => new RecipeStepInputDto(step.StepNumber, step.Duration))
                    .ToList(),
                SubRecipes: Array.Empty<RecipeInputDto>());

        private static Tasks BuildTaskFromPlan(
            PlannedPrepTaskDto plannedTask, SmartTaskRun owningRun, SmartTaskRequest originalRequest) =>
            new()
            {
                Name = plannedTask.RecipeTitle,
                Station = Station.Prep,
                Status = WorkTaskStatus.Pending,
                Assigner = "SmartTask",
                Date = DateTime.UtcNow,
                UserId = plannedTask.AssignedUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DueDate = plannedTask.DueDateUtc,
                LocationId = originalRequest.LocationId,
                Kind = WorkTaskKind.PrepFromRecipe,
                RecipeId = plannedTask.RecipeId,
                Priority = plannedTask.Priority,
                Notes = plannedTask.ReasoningSummary,
                SmartTaskRunId = owningRun.Id,
                GeneratedBy = "SmartTask"
            };
    }
}