using GymManagementSystem.DataLayer.Interfaces;

namespace GymManagementSystem.DataLayer.Entites;

public class BaseEntity : IBaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public bool IsDeleted { get; set; }
}