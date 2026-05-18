using GymManagementSystem.BusinessLayer.Interfaces;

namespace GymManagementSystem.PresentationLayer.BackgroundGobs;

public class SoftDeleteCleanUp : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SoftDeleteCleanUp> _logger;

    public SoftDeleteCleanUp(IServiceScopeFactory scopeFactory, ILogger<SoftDeleteCleanUp> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SoftDeleteCleanUp background job started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var cleanUpDeletedRows = scope.ServiceProvider.GetRequiredService<ICleanUpDeletedRows>();
                var numberOfDeletedRows = await cleanUpDeletedRows.SoftDeleteAsync(stoppingToken);

                _logger.LogInformation("Cleanup cycle completed: {DeletedRows} rows purged", numberOfDeletedRows);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "SoftDeleteCleanUp background job failed");
            }

            _logger.LogInformation("Next cleanup scheduled in 30 days");
            await Task.Delay(TimeSpan.FromDays(30), stoppingToken);
        }

        _logger.LogInformation("SoftDeleteCleanUp background job stopped");
    }
}
