namespace GymManagementSystem.BusinessLayer.DTOs.Booking;

public class BookingDto
{
    public int Id { get; set; }
    public DateTime BookingDate { get; set; }
    public bool IsAttended { get; set; }
    public DateTime? CheckedInAt { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = null!;
    public string MemberEmail { get; set; } = null!;
    public int SessionId { get; set; }
    public string SessionDescription { get; set; } = null!;
    public DateTime SessionStartDate { get; set; }
    public DateTime SessionEndDate { get; set; }
    public string TrainerName { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
