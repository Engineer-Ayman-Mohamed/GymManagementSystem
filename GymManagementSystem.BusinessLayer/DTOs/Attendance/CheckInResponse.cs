using GymManagementSystem.BusinessLayer.Enums;

namespace GymManagementSystem.BusinessLayer.DTOs.Attendance;

public class CheckInResponse
{
    public CheckInResult Result { get; set; }
    public string? MemberName { get; set; }
    public string? MemberEmail { get; set; }
    public string? SessionDescription { get; set; }
    public string? SessionDate { get; set; }
    public string? TrainerName { get; set; }
    public string? CategoryName { get; set; }
    public DateTime? CheckedInAt { get; set; }
}