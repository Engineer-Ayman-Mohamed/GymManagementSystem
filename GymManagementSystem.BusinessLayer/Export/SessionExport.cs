using GymManagementSystem.BusinessLayer.DTOs.Session;
using GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation;

namespace GymManagementSystem.BusinessLayer.Export;

public static class SessionExport
{
    public static IReadOnlyList<ColumnDefinition<SessionDto>> GetColumns() =>
    [
        new("Id", s => s.Id),
        new("Description", s => s.Description),
        new("Capacity", s => s.Capacity),
        new("Start Date", s => s.StartDate.ToString("yyyy-MM-dd HH:mm")),
        new("End Date", s => s.EndDate.ToString("yyyy-MM-dd HH:mm")),
        new("Trainer", s => s.TrainerName),
        new("Category", s => s.CategoryName),
    ];
}
