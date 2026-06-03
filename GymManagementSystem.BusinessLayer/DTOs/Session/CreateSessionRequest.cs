namespace GymManagementSystem.BusinessLayer.DTOs.Session;

public class CreateSessionRequest
{
    public string Description { get; set; } = null!;
    public int Capacity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TrainerId { get; set; }
    public int CategoryId { get; set; }
}
