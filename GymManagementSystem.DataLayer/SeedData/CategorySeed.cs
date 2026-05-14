using Bogus;
using GymManagementSystem.DataLayer.Entites;

namespace GymManagementSystem.DataLayer.SeedData;

public static class CategorySeed
{
    public static List<Category> Generate(int count = 6)
    {
        var names = new[] { "Yoga", "HIIT", "Strength", "Cardio", "Pilates", "Boxing" };

        var faker = new Faker<Category>()
            .RuleFor(c => c.CategoryName, f => f.PickRandom(names))
            .RuleFor(c => c.CreatedAt, f => f.Date.Past(1))
            .RuleFor(c => c.UpdatedAt, (_, c) => c.CreatedAt)
            .RuleFor(c => c.CreatedBy, "Seed")
            .RuleFor(c => c.IsDeleted, false);

        var categories = faker.Generate(count);

        for (int i = 0; i < categories.Count; i++)
            categories[i].CategoryName = names[i % names.Length];

        return categories.DistinctBy(c => c.CategoryName).ToList();
    }
}