namespace GymManagementSystem.DataLayer.Entites;

public class Category : BaseEntity
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = null!;

    public ICollection<Session> Sessions { get; set; }
        = new List<Session>();
}