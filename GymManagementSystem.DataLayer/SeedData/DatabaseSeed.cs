using GymManagementSystem.DataLayer.Database;

namespace GymManagementSystem.DataLayer.SeedData;

public static class DatabaseSeed
{
    public static async Task SeedAsync(GymDatabaseContext context)
    {
        if (context.Plans.Any())
            return;

        var plans = PlanSeed.Generate(5);
        var categories = CategorySeed.Generate(6);
        var members = MemberSeed.Generate(10);
        var trainers = TrainerSeed.Generate(4);

        await context.Plans.AddRangeAsync(plans);
        await context.Categories.AddRangeAsync(categories);
        await context.Members.AddRangeAsync(members);
        await context.Trainers.AddRangeAsync(trainers);
        await context.SaveChangesAsync();

        var planIds = plans.Select(p => p.Id).ToList();
        var categoryIds = categories.Select(c => c.Id).ToList();
        var memberIds = members.Select(m => m.Id).ToList();
        var trainerIds = trainers.Select(t => t.Id).ToList();

        var sessions = SessionSeed.Generate(10, trainerIds, categoryIds);
        var healthRecords = HealthRecordSeed.Generate(memberIds);
        var memberships = MembershipSeed.Generate(12, memberIds, planIds);
        var bookings = BookingSeed.Generate(15, memberIds, sessions.Select(s => s.Id).ToList());

        await context.Sessions.AddRangeAsync(sessions);
        await context.HealthRecords.AddRangeAsync(healthRecords);
        await context.Memberships.AddRangeAsync(memberships);
        await context.Bookings.AddRangeAsync(bookings);
        await context.SaveChangesAsync();
    }
}