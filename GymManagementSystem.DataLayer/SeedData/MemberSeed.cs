using Bogus;
using Bogus.DataSets;
using GymManagementSystem.DataLayer.Entites;
using GymManagementSystem.DataLayer.Enums;
using Address = GymManagementSystem.DataLayer.Entites.Address;

namespace GymManagementSystem.DataLayer.SeedData;

public static class MemberSeed
{
    public static List<Member> Generate(int count = 10)
    {
        var faker = new Faker<Member>()
            .RuleFor(m => m.Name, f => f.Name.FullName())
            .RuleFor(m => m.Email, (f, m) => f.Internet.Email(m.Name))
            .RuleFor(m => m.Phone, f =>
                f.PickRandom("010", "011", "012", "015") + f.Random.String2(8, "0123456789"))
            .RuleFor(m => m.DateOfBirth, f => DateOnly.FromDateTime(f.Date.Between(
                new DateTime(1960, 1, 1), new DateTime(2005, 12, 31))))
            .RuleFor(m => m.Gender, f => f.PickRandom<Gender>())
            .RuleFor(m => m.Address, f => new Address
            {
                BuildingNumber = f.Random.Int(1, 999),
                Street = f.Address.StreetName(),
                City = f.Address.City()
            })
            .RuleFor(m => m.Photo, f => f.Internet.Avatar())
            .RuleFor(m => m.JoinDate, f => f.Date.Past(2))
            .RuleFor(m => m.CreatedAt, f => f.Date.Past(1))
            .RuleFor(m => m.UpdatedAt, (_, m) => m.CreatedAt)
            .RuleFor(m => m.CreatedBy, "Seed")
            .RuleFor(m => m.IsDeleted, false);

        return faker.Generate(count);
    }
}