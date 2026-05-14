using System.ComponentModel.DataAnnotations;
using GymManagementSystem.DataLayer.Enums;

namespace GymManagementSystem.DataLayer.Entites;

public abstract class GymUser : BaseEntity
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(100), EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MaxLength(11)]
    [RegularExpression(@"^(010|011|012|015)\d{8}$",
        ErrorMessage = "Egyptian phone format only.")]
    public string Phone { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public Address Address { get; set; } = null!;
}