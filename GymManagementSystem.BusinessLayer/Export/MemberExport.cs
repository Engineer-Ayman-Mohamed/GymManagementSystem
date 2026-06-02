using GymManagementSystem.BusinessLayer.DTOs.Member;
using GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation;

namespace GymManagementSystem.BusinessLayer.Export;

public static class MemberExport
{
    public static IReadOnlyList<ColumnDefinition<MemberDto>> GetColumns() =>
    [
        new("Id", m => m.Id),
        new("Name", m => m.Name),
        new("Email", m => m.Email),
        new("Phone", m => m.Phone),
        new("Gender", m => m.Gender),
        new("Date Of Birth", m => m.DateOfBirth.ToString("yyyy-MM-dd")),
        new("Join Date", m => m.JoinDate.ToString("yyyy-MM-dd")),
        new("City", m => m.City),
    ];
}