namespace GymManagementSystem.BusinessLayer.DTOs.Plan;

public class CreatePlanRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int DurationDays { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}
