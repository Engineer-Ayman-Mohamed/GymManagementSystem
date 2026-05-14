using Bogus;
using GymManagementSystem.DataLayer.Entites;

namespace GymManagementSystem.DataLayer.SeedData;

public static class BookingSeed
{
    public static List<Booking> Generate(int count, List<int> memberIds, List<int> sessionIds)
    {
        var faker = new Faker<Booking>()
            .RuleFor(b => b.BookingDate, f => f.Date.Recent(30))
            .RuleFor(b => b.IsAttended, f => f.Random.Bool(0.7f))
            .RuleFor(b => b.MemberId, f => f.PickRandom(memberIds))
            .RuleFor(b => b.SessionId, f => f.PickRandom(sessionIds))
            .RuleFor(b => b.CreatedAt, f => f.Date.Past(1))
            .RuleFor(b => b.UpdatedAt, (_, b) => b.CreatedAt)
            .RuleFor(b => b.CreatedBy, "Seed")
            .RuleFor(b => b.IsDeleted, false);

        return faker.Generate(count);
    }
}