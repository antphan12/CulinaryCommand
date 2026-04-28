using System.Net.Http.Json;
using System.Text.Json;
using CulinaryCommandApp.SmartTask.Services.Interfaces;

namespace CulinaryCommandApp.SmartTask.Services
{
    public sealed class SmartTaskLambdaClient : ISmartTaskOrchestratorClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly HttpClient _httpClient;

        public SmartTaskLambdaClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PlanResponseDto> RequestPlanAsync(
            PlanRequestDto planRequest, CancellationToken cancellationToken)
        {
            try
            {
                var lambdaResponse = await _httpClient.PostAsJsonAsync(
                    string.Empty, planRequest, JsonOptions, cancellationToken);

                lambdaResponse.EnsureSuccessStatusCode();

                var deserializedPlan = await lambdaResponse.Content.ReadFromJsonAsync<PlanResponseDto>(
                    JsonOptions, cancellationToken);

                return deserializedPlan
                    ?? throw new InvalidOperationException("Lambda returned an empty plan body.");
            }
            catch (Exception failure) when (failure is HttpRequestException or TaskCanceledException)
            {
                throw new SmartTaskOrchestratorUnavailableException(
                    "SmartTask planner is currently unavailable.", failure);
            }
        }
    }
}