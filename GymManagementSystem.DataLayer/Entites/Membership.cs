namespace GymManagementSystem.DataLayer.Entites;

public class Membership : BaseEntity
{
    public int Id { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate   { get; set; }

    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public int PlanId { get; set; }
    public Plan Plan { get; set; } = null!;
}