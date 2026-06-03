namespace GymManagementSystem.BusinessLayer.DTOs.Membership;

public class CreateMembershipRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MemberId { get; set; }
    public int PlanId { get; set; }
}
