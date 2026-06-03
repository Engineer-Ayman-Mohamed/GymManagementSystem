namespace GymManagementSystem.BusinessLayer.DTOs.Session;

public class SessionDto
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public int Capacity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TrainerId { get; set; }
    public string TrainerName { get; set; } = null!;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
