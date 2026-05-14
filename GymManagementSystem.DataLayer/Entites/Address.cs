using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DataLayer.Entites;

[Owned]
public class Address
{
    public int BuildingNumber { get; set; }

    [Required, MaxLength(30)]
    public string Street { get; set; } = null!;

    [Required, MaxLength(30)]
    public string City { get; set; } = null!;
}
