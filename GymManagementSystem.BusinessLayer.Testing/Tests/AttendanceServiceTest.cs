using GymManagementSystem.BusinessLayer.Enums;
using GymManagementSystem.BusinessLayer.Helpers.Attendance;
using GymManagementSystem.BusinessLayer.Services;
using GymManagementSystem.DataLayer.Entites;
using GymManagementSystem.DataLayer.Interfaces;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace GymManagementSystem.BusinessLayer.Testing.Tests;

public class AttendanceServiceTest
{
    private readonly IGenericRepository<Booking> _fakeRepo;
    private readonly ILogger<AttendanceService> _fakeLogger;
    private readonly AttendanceService _service;
    private const string SecretKey = "test-secret-key-12345";
    public AttendanceServiceTest()
    {
        _fakeRepo = Substitute.For<IGenericRepository<Booking>>();
        _fakeLogger = Substitute.For<ILogger<AttendanceService>>();
        _service = new AttendanceService(_fakeRepo, _fakeLogger);
    }

    private static Booking CreateFakeBooking(
        int id = 1,
        bool isAttended = false,
        DateTime? sessionStartDate = null
    ) {
        return new Booking
        {
            Id = id,
            IsAttended = isAttended,
            Member = new Member { Id = 1, Name = "Test Member", Email = "test@email.com" },
            Session = new Session
            {
                Id = 1,
                Description = "Morning Yoga",
                StartDate = sessionStartDate ?? DateTime.UtcNow.Date,
                Trainer = new Trainer { Id = 1, Name = "Coach Ali" },
                Category = new Category { Id = 1, CategoryName = "Yoga" }
            }
        };
    }

    [Fact]
    public async Task U5_CheckIn_Success_ShouldMarkAsAttended()
    {
        var payload = _service.BuildPayload(1, SecretKey);

        var booking = CreateFakeBooking(id: 1, isAttended: false);
        _fakeRepo.GetByIdWithIncludesAsync(
                1, Arg.Any<CancellationToken>(),
                Arg.Any<System.Linq.Expressions.Expression<Func<Booking, object>>[]>())
            .Returns(booking);

        var result = await _service.CheckInAsync(payload, SecretKey);

        result.Result.ShouldBe(CheckInResult.Success);
        result.MemberName.ShouldBe("Test Member");
        result.SessionDescription.ShouldBe("Morning Yoga");

        booking.IsAttended.ShouldBeTrue();
        booking.CheckedInAt.ShouldNotBeNull();

        await _fakeRepo.Received(1).UpdateAsync(
            Arg.Any<Booking>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task U6_CheckIn_InvalidSignature_ShouldReject()
    {
        var payload = _service.BuildPayload(1, SecretKey);
        var tamperedPayload = payload.Substring(0, payload.Length - 5) + "XXXXX";

        var result = await _service.CheckInAsync(tamperedPayload, SecretKey);

        result.Result.ShouldBe(CheckInResult.InvalidSignature);

        await _fakeRepo.DidNotReceive().GetByIdWithIncludesAsync(
            Arg.Any<int>(), Arg.Any<CancellationToken>(),
            Arg.Any<System.Linq.Expressions.Expression<Func<Booking, object>>[]>());
    }

    [Fact]
    public async Task U7_CheckIn_AlreadyAttended_ShouldReject()
    {
        var payload = _service.BuildPayload(1, SecretKey);
        var booking = CreateFakeBooking(id: 1, isAttended: true);
        _fakeRepo.GetByIdWithIncludesAsync(
                1, Arg.Any<CancellationToken>(),
                Arg.Any<System.Linq.Expressions.Expression<Func<Booking, object>>[]>())
            .Returns(booking);

        var result = await _service.CheckInAsync(payload, SecretKey);

        result.Result.ShouldBe(CheckInResult.AlreadyAttended);
        result.MemberName.ShouldBe("Test Member");

        await _fakeRepo.DidNotReceive().UpdateAsync(
            Arg.Any<Booking>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task U8_CheckIn_BookingNotFound_ShouldReturnNotFound()
    {
        var payload = _service.BuildPayload(99999, SecretKey);
        _fakeRepo.GetByIdWithIncludesAsync(
                99999, Arg.Any<CancellationToken>(),
                Arg.Any<System.Linq.Expressions.Expression<Func<Booking, object>>[]>())
            .ReturnsNull();

        var result = await _service.CheckInAsync(payload, SecretKey);

        result.Result.ShouldBe(CheckInResult.NotFound);
        await _fakeRepo.DidNotReceive().UpdateAsync(
            Arg.Any<Booking>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task U9_CheckIn_SessionNotToday_ShouldReject()
    {
        var payload = _service.BuildPayload(1, SecretKey);
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var booking = CreateFakeBooking(id: 1, isAttended: false, sessionStartDate: yesterday);
        _fakeRepo.GetByIdWithIncludesAsync(
                1, Arg.Any<CancellationToken>(),
                Arg.Any<System.Linq.Expressions.Expression<Func<Booking, object>>[]>())
            .Returns(booking);

        var result = await _service.CheckInAsync(payload, SecretKey);

        result.Result.ShouldBe(CheckInResult.SessionNotToday);
        result.SessionDate.ShouldNotBeNull();
        booking.IsAttended.ShouldBeFalse();
    }

    [Fact]
    public async Task U10_PayloadRoundTrip_BuildThenCheckIn_ShouldSucceed()
    {
        var payload = _service.BuildPayload(1, SecretKey);
        var booking = CreateFakeBooking(id: 1, isAttended: false);
        _fakeRepo.GetByIdWithIncludesAsync(
                1, Arg.Any<CancellationToken>(),
                Arg.Any<System.Linq.Expressions.Expression<Func<Booking, object>>[]>())
            .Returns(booking);

        var result = await _service.CheckInAsync(payload, SecretKey);

        result.Result.ShouldBe(CheckInResult.Success);
    }

    [Fact]
    public void U10_InvalidFormat_RandomString_ShouldReturnInvalidFormat()
    {
        var invalidPayload = "this is garbage data";

        var parsed = PayloadHelper.Parse(invalidPayload);

        parsed.ShouldBeNull();
    }
}