using Bogus;
using GymManagementSystem.DataLayer.Entites;

namespace GymManagementSystem.DataLayer.SeedData;

public static class MembershipSeed
{
    public static List<Membership> Generate(int count, List<int> memberIds, List<int> planIds)
    {
        var faker = new Faker<Membership>()
            .RuleFor(m => m.StartDate, f => f.Date.Past(1))
            .RuleFor(m => m.EndDate, (f, m) => m.StartDate.AddDays(f.Random.Int(30, 365)))
            .RuleFor(m => m.MemberId, f => f.PickRandom(memberIds))
            .RuleFor(m => m.PlanId, f => f.PickRandom(planIds))
            .RuleFor(m => m.CreatedAt, f => f.Date.Past(1))
            .RuleFor(m => m.UpdatedAt, (_, m) => m.CreatedAt)
            .RuleFor(m => m.CreatedBy, "Seed")
            .RuleFor(m => m.IsDeleted, false);

        return faker.Generate(count);
    }
}