using GymManagementSystem.BusinessLayer.DTOs.Trainer;
using GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation;

namespace GymManagementSystem.BusinessLayer.Export;

public static class TrainerExport
{
    public static IReadOnlyList<ColumnDefinition<TrainerDto>> GetColumns() =>
    [
        new("Id", t => t.Id),
        new("Name", t => t.Name),
        new("Email", t => t.Email),
        new("Phone", t => t.Phone),
        new("Gender", t => t.Gender),
        new("Date Of Birth", t => t.DateOfBirth.ToString("yyyy-MM-dd")),
        new("Specialty", t => t.Specialty),
        new("Hire Date", t => t.HireDate.ToString("yyyy-MM-dd")),
        new("City", t => t.City),
    ];
}
