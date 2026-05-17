using GymManagementSystem.BusinessLayer.Interfaces;
using GymManagementSystem.BusinessLayer.Services;

namespace GymManagementSystem.PresentationLayer.BackgroundGobs;
/// <summary>
/// main execution of the remove soft delete rows in main database logic using the DI container
/// </summary>
public class SoftDeleteCleanUp : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CleanUpDeletedRowsServices> _logger;

    public SoftDeleteCleanUp(IServiceScopeFactory scopeFactory,
        ILogger<CleanUpDeletedRowsServices> logger
        )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var cleanUpDeletedRows = scope.ServiceProvider.GetService<ICleanUpDeletedRows>();
                var numberOfDeletedRows = await cleanUpDeletedRows?.SoftDeleteAsync(stoppingToken)!;
                _logger.LogInformation($"numberOfDeletedRows: {numberOfDeletedRows} from the CleanUpDeletedRows service");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Clean Up Deleted Rows Service Exception");
            }
            await  Task.Delay(TimeSpan.FromDays(30), stoppingToken);
        }
    }
}