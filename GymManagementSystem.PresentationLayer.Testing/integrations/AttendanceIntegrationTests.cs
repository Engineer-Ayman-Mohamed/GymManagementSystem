using System.Net;
using GymManagementSystem.BusinessLayer.Helpers.Attendance;
using GymManagementSystem.DataLayer.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace GymManagementSystem.PresentationLayer.Testing.integrations;

public class AttendanceIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;


    public AttendanceIntegrationTests(CustomWebApplicationFactory factory)
    {
        var options = new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };
        _client = factory.CreateClient(options);
        _factory = factory;
    }

    [Fact]
    public async Task I4_CheckIn_ValidPayload_ShouldMarkBookingAsAttended()
    {
        int bookingId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymDatabaseContext>();
            bookingId = db.Bookings.First().Id;
        }

        var payload = PayloadHelper.BuildPayload(
            bookingId, PayloadHelper.ComputeSignature(bookingId, CustomWebApplicationFactory.TestSecretKey));

        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("payload", payload)
        });
        var response = await _client.PostAsync("/Attendance/CheckIn", formData);

        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymDatabaseContext>();
            var booking = db.Bookings.First(b => b.Id == bookingId);
            booking.IsAttended.ShouldBeTrue();
            booking.CheckedInAt.ShouldNotBeNull();
        }
    }

    [Fact]
    public async Task I5_CheckIn_InvalidSignature_ShouldNotMarkAttended()
    {
        int bookingId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymDatabaseContext>();
            bookingId = db.Bookings.Skip(1).First().Id;
        }

        var badPayload = $"GYMYCHECKIN:{bookingId}:WRONGSIG";

        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("payload", badPayload)
        });
        var response = await _client.PostAsync("/Attendance/CheckIn", formData);

        response.StatusCode.ShouldNotBe(HttpStatusCode.InternalServerError);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymDatabaseContext>();
            var booking = db.Bookings.First(b => b.Id == bookingId);
            booking.IsAttended.ShouldBeFalse();
        }
    }

    [Fact]
    public async Task I6_ErrorPage_ShouldReturn200()
    {
        var response = await _client.GetAsync("/Home/Error");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.ShouldNotBeNullOrWhiteSpace();
    }
}
