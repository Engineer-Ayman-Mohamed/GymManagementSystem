using System.Linq.Expressions;
using GymManagementSystem.DataLayer.Entites;

namespace GymManagementSystem.DataLayer.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<T?> GetByIdWithIncludesAsync(int id, CancellationToken ct = default, params Expression<Func<T, object>>[] includes);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllWithIncludesAsync(CancellationToken ct = default, params Expression<Func<T, object>>[] includes);
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task SoftDeleteAsync(T entity, CancellationToken ct = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
