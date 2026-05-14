namespace GymManagementSystem.DataLayer.Interfaces;

public interface IBaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public bool IsDeleted { get; set; }
}