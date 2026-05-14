using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.DataLayer.Entites;

public class HealthRecord : BaseEntity
{
    public int Id { get; set; }
    public decimal Height    { get; set; }  
    public decimal Weight    { get; set; }   
    
    [Required, MaxLength(5)]
    public string BloodType { get; set; } = null!;

    [MaxLength(500)]
    public string? Note { get; set; }

    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;
}