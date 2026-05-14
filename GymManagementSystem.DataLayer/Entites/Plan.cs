using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagementSystem.DataLayer.Entites;


public class Plan : BaseEntity
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Description { get; set; } = null!;

    [Range(1, 365)]
    public int DurationDays { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;   

    public ICollection<Membership> Memberships { get; set; }
        = new List<Membership>();
}