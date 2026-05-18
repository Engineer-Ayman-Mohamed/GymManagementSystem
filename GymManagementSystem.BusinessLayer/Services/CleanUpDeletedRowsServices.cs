using GymManagementSystem.BusinessLayer.Interfaces;
using GymManagementSystem.DataLayer.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.BusinessLayer.Services;

public class CleanUpDeletedRowsServices : ICleanUpDeletedRows
{
    private readonly GymDatabaseContext _context;
    private readonly ILogger<CleanUpDeletedRowsServices> _logger;

    public CleanUpDeletedRowsServices(GymDatabaseContext context, ILogger<CleanUpDeletedRowsServices> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> SoftDeleteAsync(CancellationToken ct = default)
    {
        var numberOfDeletedRows = 0;

        numberOfDeletedRows += await DeleteFromTableAsync("Members", _context.Members, ct);
        numberOfDeletedRows += await DeleteFromTableAsync("Trainers", _context.Trainers, ct);
        numberOfDeletedRows += await DeleteFromTableAsync("Sessions", _context.Sessions, ct);
        numberOfDeletedRows += await DeleteFromTableAsync("Bookings", _context.Bookings, ct);
        numberOfDeletedRows += await DeleteFromTableAsync("HealthRecords", _context.HealthRecords, ct);
        numberOfDeletedRows += await DeleteFromTableAsync("Plans", _context.Plans, ct);
        numberOfDeletedRows += await DeleteFromTableAsync("Categories", _context.Categories, ct);
        numberOfDeletedRows += await DeleteFromTableAsync("Memberships", _context.Memberships, ct);

        _logger.LogInformation("Cleanup completed: {TotalRows} soft-deleted rows purged across all tables", numberOfDeletedRows);

        return numberOfDeletedRows;
    }

    private async Task<int> DeleteFromTableAsync<T>(string tableName, DbSet<T> dbSet, CancellationToken ct) where T : class
    {
        var count = await dbSet.IgnoreQueryFilters().Where(e => EF.Property<bool>(e, "IsDeleted")).ExecuteDeleteAsync(ct);
        if (count > 0)
        {
            _logger.LogInformation("Purged {DeletedCount} soft-deleted rows from {TableName}", count, tableName);
        }
        return count;
    }
}
