namespace GymManagementSystem.DataLayer.Entites;

public class Booking : BaseEntity
{
    public int Id { get; set; }
    public DateTime BookingDate { get; set; }
    public bool IsAttended { get; set; } = false;
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;
    public int SessionId { get; set; }
    public Session Session { get; set; } = null!;
    public DateTime? CheckedInAt { get; set; }
}