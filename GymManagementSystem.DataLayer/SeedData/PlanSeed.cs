using Bogus;
using GymManagementSystem.DataLayer.Entites;

namespace GymManagementSystem.DataLayer.SeedData;

public static class PlanSeed
{
    public static List<Plan> Generate(int count = 5)
    {
        var planNames = new[] { "Starter", "Basic", "Premium", "Gold", "Platinum" };

        var faker = new Faker<Plan>()
            .RuleFor(p => p.Name, f => f.PickRandom(planNames))
            .RuleFor(p => p.Description, f => f.Lorem.Sentence(5))
            .RuleFor(p => p.DurationDays, f => f.PickRandom(30, 60, 90, 180, 365))
            .RuleFor(p => p.Price, f => Math.Round(f.Random.Decimal(50, 500), 2))
            .RuleFor(p => p.IsActive, true)
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(1))
            .RuleFor(p => p.UpdatedAt, (_, p) => p.CreatedAt)
            .RuleFor(p => p.CreatedBy, "Seed")
            .RuleFor(p => p.IsDeleted, false);

        return faker.Generate(count);
    }
}