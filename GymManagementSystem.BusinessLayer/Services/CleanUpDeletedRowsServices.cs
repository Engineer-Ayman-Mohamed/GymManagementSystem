using GymManagementSystem.BusinessLayer.Interfaces;
using GymManagementSystem.DataLayer.Database;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.BusinessLayer.Services;
/// <summary>
/// the main logic to clean up deleted rows
/// </summary>
public class CleanUpDeletedRowsServices : ICleanUpDeletedRows
{
    private readonly GymDatabaseContext _context;
    public CleanUpDeletedRowsServices(GymDatabaseContext context)
    {
        _context = context;
    }
    public async Task<int> SoftDeleteAsync(CancellationToken ct = default)
    {
        var numberOfDeletedRows = 0;
        numberOfDeletedRows += await _context.Members.IgnoreQueryFilters().Where(e => e.IsDeleted).ExecuteDeleteAsync(ct);
        numberOfDeletedRows += await _context.Trainers.IgnoreQueryFilters().Where(e => e.IsDeleted).ExecuteDeleteAsync(ct);
        numberOfDeletedRows += await _context.Sessions.IgnoreQueryFilters().Where(e => e.IsDeleted).ExecuteDeleteAsync(ct);
        numberOfDeletedRows += await _context.Bookings.IgnoreQueryFilters().Where(e => e.IsDeleted).ExecuteDeleteAsync(ct);
        numberOfDeletedRows += await _context.HealthRecords.IgnoreQueryFilters().Where(e => e.IsDeleted).ExecuteDeleteAsync(ct);
        numberOfDeletedRows += await _context.Plans.IgnoreQueryFilters().Where(e => e.IsDeleted).ExecuteDeleteAsync(ct);
        numberOfDeletedRows += await _context.Categories.IgnoreQueryFilters().Where(e => e.IsDeleted).ExecuteDeleteAsync(ct);
        numberOfDeletedRows += await _context.Memberships.IgnoreQueryFilters().Where(e => e.IsDeleted).ExecuteDeleteAsync(ct);
        return numberOfDeletedRows;
    }
}