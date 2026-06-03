namespace GymManagementSystem.BusinessLayer.DTOs.Membership;

public class MembershipDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = null!;
    public string MemberEmail { get; set; } = null!;
    public int PlanId { get; set; }
    public string PlanName { get; set; } = null!;
    public decimal PlanPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}
