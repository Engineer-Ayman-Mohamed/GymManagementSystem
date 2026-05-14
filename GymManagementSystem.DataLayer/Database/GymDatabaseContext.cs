using GymManagementSystem.DataLayer.Entites;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DataLayer.Database;

public class GymDatabaseContext : DbContext
{
    public GymDatabaseContext(DbContextOptions<GymDatabaseContext> options)
        : base(options) { }

    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Trainer> Trainers => Set<Trainer>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<HealthRecord> HealthRecords => Set<HealthRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Gym");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymDatabaseContext).Assembly);
    }
}