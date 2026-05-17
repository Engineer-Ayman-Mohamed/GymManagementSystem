namespace GymManagementSystem.BusinessLayer.Interfaces;

public interface ICleanUpDeletedRows
{
    Task<int> SoftDeleteAsync(CancellationToken ct = default);
}