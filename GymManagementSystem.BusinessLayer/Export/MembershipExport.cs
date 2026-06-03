using GymManagementSystem.BusinessLayer.DTOs.Membership;
using GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation;

namespace GymManagementSystem.BusinessLayer.Export;

public static class MembershipExport
{
    public static IReadOnlyList<ColumnDefinition<MembershipDto>> GetColumns() =>
    [
        new("Id", m => m.Id),
        new("Member", m => m.MemberName),
        new("Member Email", m => m.MemberEmail),
        new("Plan", m => m.PlanName),
        new("Plan Price", m => m.PlanPrice),
        new("Start Date", m => m.StartDate.ToString("yyyy-MM-dd")),
        new("End Date", m => m.EndDate.ToString("yyyy-MM-dd")),
        new("Created At", m => m.CreatedAt.ToString("yyyy-MM-dd HH:mm")),
    ];
}
