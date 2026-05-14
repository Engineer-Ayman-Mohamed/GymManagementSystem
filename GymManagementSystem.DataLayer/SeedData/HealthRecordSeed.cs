using Bogus;
using GymManagementSystem.DataLayer.Entites;

namespace GymManagementSystem.DataLayer.SeedData;

public static class HealthRecordSeed
{
    private static readonly string[] BloodTypes = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };

    public static List<HealthRecord> Generate(List<int> memberIds)
    {
        var faker = new Faker<HealthRecord>()
            .RuleFor(h => h.Height, f => Math.Round(f.Random.Decimal(150, 200), 2))
            .RuleFor(h => h.Weight, f => Math.Round(f.Random.Decimal(50, 120), 2))
            .RuleFor(h => h.BloodType, f => f.PickRandom(BloodTypes))
            .RuleFor(h => h.Note, f => f.Random.Bool(0.3f) ? f.Lorem.Sentence() : null)
            .RuleFor(h => h.MemberId, f => f.PickRandom(memberIds))
            .RuleFor(h => h.CreatedAt, f => f.Date.Past(1))
            .RuleFor(h => h.UpdatedAt, (_, h) => h.CreatedAt)
            .RuleFor(h => h.CreatedBy, "Seed")
            .RuleFor(h => h.IsDeleted, false);

        var records = faker.Generate(memberIds.Count);
        for (int i = 0; i < records.Count; i++)
            records[i].MemberId = memberIds[i];

        return records;
    }
}