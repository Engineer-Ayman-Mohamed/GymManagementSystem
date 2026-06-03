namespace GymManagementSystem.BusinessLayer.DTOs.HealthRecord;

public class UpdateHealthRecordRequest
{
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public string? BloodType { get; set; }
    public string? Note { get; set; }
}
