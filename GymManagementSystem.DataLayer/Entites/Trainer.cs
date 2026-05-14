using GymManagementSystem.DataLayer.Enums;

namespace GymManagementSystem.DataLayer.Entites;

public class Trainer : GymUser
{
    public int Id { get; set; }

    public Specialty Specialty { get; set; }
    public DateTime HireDate { get; set; }

    public ICollection<Session> Sessions { get; set; }
        = new List<Session>();
}