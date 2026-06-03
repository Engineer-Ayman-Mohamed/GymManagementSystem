using GymManagementSystem.BusinessLayer.Enums;
using GymManagementSystem.BusinessLayer.Export;
using GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation;

namespace GymManagementSystem.BusinessLayer.Interfaces;

/// <summary>
/// this contract is simply give the export service an method to have the implementation of file and
/// return back the file as bytes
/// Ex: "Take my data, map it to columns, and export it as Excel or PDF — return the result as a downloadable file."
/// </summary>
public interface IExportService
{
    Task<byte[]> ExportAsync<T>(
        IReadOnlyList<T> rows,
        IReadOnlyList<ColumnDefinition<T>> columns,
        ExportFormat format,
        string title,
        CancellationToken ct = default
    );
}