using GymManagementSystem.BusinessLayer.DTOs.Booking;

namespace GymManagementSystem.BusinessLayer.Interfaces;

public interface IBookingService
{
    Task<IReadOnlyList<BookingDto>> GetAllAsync(CancellationToken ct = default);
    Task<BookingDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<BookingDto> CreateAsync(CreateBookingRequest request, CancellationToken ct = default);
    Task<BookingDto> UpdateAsync(int id, UpdateBookingRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
