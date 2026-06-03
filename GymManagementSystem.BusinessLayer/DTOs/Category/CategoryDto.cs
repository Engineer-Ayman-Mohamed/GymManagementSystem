namespace GymManagementSystem.BusinessLayer.DTOs.Category;

public class CategoryDto
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
