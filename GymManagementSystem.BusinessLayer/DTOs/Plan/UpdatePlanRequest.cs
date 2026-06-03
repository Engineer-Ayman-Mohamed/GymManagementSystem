namespace GymManagementSystem.BusinessLayer.DTOs.Plan;

public class UpdatePlanRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? DurationDays { get; set; }
    public decimal? Price { get; set; }
    public bool? IsActive { get; set; }
}
