using Bogus;
using GymManagementSystem.DataLayer.Entites;
using GymManagementSystem.DataLayer.Enums;
using Address = GymManagementSystem.DataLayer.Entites.Address;

namespace GymManagementSystem.DataLayer.SeedData;

public static class TrainerSeed
{
    public static List<Trainer> Generate(int count = 4)
    {
        var faker = new Faker<Trainer>()
            .RuleFor(t => t.Name, f => f.Name.FullName())
            .RuleFor(t => t.Email, (f, t) => f.Internet.Email(t.Name))
            .RuleFor(t => t.Phone, f =>
                f.PickRandom("010", "011", "012", "015") + f.Random.String2(8, "0123456789"))
            .RuleFor(t => t.DateOfBirth, f => DateOnly.FromDateTime(f.Date.Between(
                new DateTime(1960, 1, 1), new DateTime(2000, 12, 31))))
            .RuleFor(t => t.Gender, f => f.PickRandom<Gender>())
            .RuleFor(t => t.Address, f => new Address
            {
                BuildingNumber = f.Random.Int(1, 999),
                Street = f.Address.StreetName(),
                City = f.Address.City()
            })
            .RuleFor(t => t.Specialty, f => f.PickRandom<Specialty>())
            .RuleFor(t => t.HireDate, f => f.Date.Past(5))
            .RuleFor(t => t.CreatedAt, f => f.Date.Past(1))
            .RuleFor(t => t.UpdatedAt, (_, t) => t.CreatedAt)
            .RuleFor(t => t.CreatedBy, "Seed")
            .RuleFor(t => t.IsDeleted, false);

        return faker.Generate(count);
    }
}