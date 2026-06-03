using GymManagementSystem.BusinessLayer.DTOs.Booking;
using GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation;

namespace GymManagementSystem.BusinessLayer.Export;

public static class BookingExport
{
    public static IReadOnlyList<ColumnDefinition<BookingDto>> GetColumns() =>
    [
        new("Id", b => b.Id),
        new("Member", b => b.MemberName),
        new("Email", b => b.MemberEmail),
        new("Session", b => b.SessionDescription),
        new("Trainer", b => b.TrainerName),
        new("Category", b => b.CategoryName),
        new("Booking Date", b => b.BookingDate.ToString("yyyy-MM-dd")),
        new("Status", b => b.IsAttended ? "Checked In" : "Pending"),
        new("Checked In At", b => b.CheckedInAt?.ToString("yyyy-MM-dd HH:mm") ?? ""),
    ];
}
