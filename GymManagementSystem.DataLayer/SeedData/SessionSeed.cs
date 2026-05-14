using Bogus;
using GymManagementSystem.DataLayer.Entites;

namespace GymManagementSystem.DataLayer.SeedData;

public static class SessionSeed
{
    public static List<Session> Generate(int count, List<int> trainerIds, List<int> categoryIds)
    {
        var faker = new Faker<Session>()
            .RuleFor(s => s.Description, f => f.Lorem.Sentence(4))
            .RuleFor(s => s.Capacity, f => f.Random.Int(5, 30))
            .RuleFor(s => s.StartDate, f => f.Date.Soon(30))
            .RuleFor(s => s.EndDate, (f, s) => s.StartDate.AddHours(f.Random.Int(1, 3)))
            .RuleFor(s => s.TrainerId, f => f.PickRandom(trainerIds))
            .RuleFor(s => s.CategoryId, f => f.PickRandom(categoryIds))
            .RuleFor(s => s.CreatedAt, f => f.Date.Past(1))
            .RuleFor(s => s.UpdatedAt, (_, s) => s.CreatedAt)
            .RuleFor(s => s.CreatedBy, "Seed")
            .RuleFor(s => s.IsDeleted, false);

        return faker.Generate(count);
    }
}