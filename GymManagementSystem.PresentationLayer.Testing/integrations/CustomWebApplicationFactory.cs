using GymManagementSystem.DataLayer.Database;
using GymManagementSystem.DataLayer.Entites;
using GymManagementSystem.DataLayer.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagementSystem.PresentationLayer.Testing.integrations;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"GymTestDb_{Guid.NewGuid()}";

    public const string TestSecretKey = "test-secret-key-12345";

    public CustomWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.Sources.Clear();
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Attendance:SecretKey"] = TestSecretKey,
                ["ConnectionStrings:SqlServer"] = "Server=localhost;Database=Fake;Trusted_Connection=True;"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.AddDbContext<GymDatabaseContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<GymDatabaseContext>();
            db.Database.EnsureCreated();
            SeedTestData(db);
        });
    }

    private static void SeedTestData(GymDatabaseContext db)
    {
        if (db.Members.Any()) return;

        var member = new Member
        {
            Name = "Test Member",
            Email = "test@email.com",
            Phone = "01012345678",
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1990, 1, 1),
            JoinDate = DateTime.UtcNow.Date,
            Address = new Address { BuildingNumber = 1, Street = "Test Street", City = "Cairo" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };
        db.Members.Add(member);

        var trainer = new Trainer
        {
            Name = "Coach Ali",
            Email = "ali@gym.com",
            Phone = "01112223333",
            Gender = Gender.Male,
            Specialty = Specialty.Yoga,
            HireDate = DateTime.UtcNow.Date,
            Address = new Address { BuildingNumber = 5, Street = "Gym Street", City = "Alex" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };
        db.Trainers.Add(trainer);

        var category = new Category
        {
            CategoryName = "Yoga",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };
        db.Categories.Add(category);

        db.SaveChanges();

        var session = new Session
        {
            Description = "Morning Yoga Session",
            Capacity = 20,
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(1),
            TrainerId = trainer.Id,
            CategoryId = category.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };
        db.Sessions.Add(session);
        db.SaveChanges();

        var booking = new Booking
        {
            BookingDate = DateTime.UtcNow,
            IsAttended = false,
            MemberId = member.Id,
            SessionId = session.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };
        db.Bookings.Add(booking);

        var session2 = new Session
        {
            Description = "Evening CrossFit Session",
            Capacity = 15,
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(1),
            TrainerId = trainer.Id,
            CategoryId = category.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };
        db.Sessions.Add(session2);
        db.SaveChanges();

        var booking2 = new Booking
        {
            BookingDate = DateTime.UtcNow,
            IsAttended = false,
            MemberId = member.Id,
            SessionId = session2.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };
        db.Bookings.Add(booking2);
        db.SaveChanges();
    }
}
