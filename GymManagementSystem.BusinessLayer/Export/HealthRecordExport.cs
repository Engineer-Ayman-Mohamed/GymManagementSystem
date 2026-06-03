using GymManagementSystem.BusinessLayer.DTOs.HealthRecord;
using GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation;

namespace GymManagementSystem.BusinessLayer.Export;

public static class HealthRecordExport
{
    public static IReadOnlyList<ColumnDefinition<HealthRecordDto>> GetColumns() =>
    [
        new("Id", r => r.Id),
        new("Member", r => r.MemberName),
        new("Height", r => r.Height),
        new("Weight", r => r.Weight),
        new("Blood Type", r => r.BloodType),
        new("Note", r => r.Note ?? ""),
        new("Created At", r => r.CreatedAt.ToString("yyyy-MM-dd HH:mm")),
    ];
}
