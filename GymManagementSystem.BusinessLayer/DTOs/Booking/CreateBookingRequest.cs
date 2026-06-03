namespace GymManagementSystem.BusinessLayer.DTOs.Booking;

public class CreateBookingRequest
{
    public DateTime BookingDate { get; set; }
    public int MemberId { get; set; }
    public int SessionId { get; set; }
}
