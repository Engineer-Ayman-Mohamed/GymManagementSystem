using GymManagementSystem.DataLayer.Database;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DataLayer.SeedData;

public static class DatabaseSeed
{
    public static async Task SeedAsync(GymDatabaseContext context)
    {
        List<int> planIds, categoryIds, memberIds, trainerIds;

        if (!await context.Plans.AnyAsync())
        {
            var plans = PlanSeed.Generate(5);
            await context.Plans.AddRangeAsync(plans);
            await context.SaveChangesAsync();
            planIds = plans.Select(p => p.Id).ToList();
        }
        else
        {
            planIds = await context.Plans.Select(p => p.Id).ToListAsync();
        }

        if (!await context.Categories.AnyAsync())
        {
            var categories = CategorySeed.Generate(6);
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
            categoryIds = categories.Select(c => c.Id).ToList();
        }
        else
        {
            categoryIds = await context.Categories.Select(c => c.Id).ToListAsync();
        }

        if (!await context.Members.AnyAsync())
        {
            var members = MemberSeed.Generate(10);
            await context.Members.AddRangeAsync(members);
            await context.SaveChangesAsync();
            memberIds = members.Select(m => m.Id).ToList();
        }
        else
        {
            memberIds = await context.Members.Select(m => m.Id).ToListAsync();
        }

        if (!await context.Trainers.AnyAsync())
        {
            var trainers = TrainerSeed.Generate(4);
            await context.Trainers.AddRangeAsync(trainers);
            await context.SaveChangesAsync();
            trainerIds = trainers.Select(t => t.Id).ToList();
        }
        else
        {
            trainerIds = await context.Trainers.Select(t => t.Id).ToListAsync();
        }

        if (!await context.Sessions.AnyAsync())
        {
            var sessions = SessionSeed.Generate(10, trainerIds, categoryIds);
            await context.Sessions.AddRangeAsync(sessions);
            await context.SaveChangesAsync();
        }

        if (!await context.HealthRecords.AnyAsync())
        {
            var healthRecords = HealthRecordSeed.Generate(memberIds);
            await context.HealthRecords.AddRangeAsync(healthRecords);
        }

        if (!await context.Memberships.AnyAsync())
        {
            var memberships = MembershipSeed.Generate(12, memberIds, planIds);
            await context.Memberships.AddRangeAsync(memberships);
        }

        if (!await context.Bookings.AnyAsync())
        {
            var sessionIds = await context.Sessions.Select(s => s.Id).ToListAsync();
            var bookings = BookingSeed.Generate(15, memberIds, sessionIds);
            await context.Bookings.AddRangeAsync(bookings);
        }

        if (!await context.HealthRecords.AnyAsync()
            || !await context.Memberships.AnyAsync()
            || !await context.Bookings.AnyAsync())
        {
            await context.SaveChangesAsync();
        }
    }
}