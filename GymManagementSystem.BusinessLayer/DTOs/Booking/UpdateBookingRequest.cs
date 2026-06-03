namespace GymManagementSystem.BusinessLayer.DTOs.Booking;

public class UpdateBookingRequest
{
    public DateTime? BookingDate { get; set; }
    public bool? IsAttended { get; set; }
    public DateTime? CheckedInAt { get; set; }
    public int? MemberId { get; set; }
    public int? SessionId { get; set; }
}
