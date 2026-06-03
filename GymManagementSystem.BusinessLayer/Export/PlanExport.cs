using GymManagementSystem.BusinessLayer.DTOs.Plan;
using GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation;

namespace GymManagementSystem.BusinessLayer.Export;

public static class PlanExport
{
    public static IReadOnlyList<ColumnDefinition<PlanDto>> GetColumns() =>
    [
        new("Id", p => p.Id),
        new("Name", p => p.Name),
        new("Description", p => p.Description),
        new("Duration Days", p => p.DurationDays),
        new("Price", p => p.Price),
        new("Is Active", p => p.IsActive ? "Yes" : "No"),
        new("Created At", p => p.CreatedAt.ToString("yyyy-MM-dd HH:mm")),
    ];
}
