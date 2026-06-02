using GymManagementSystem.BusinessLayer.Enums;
using GymManagementSystem.BusinessLayer.Export;
using GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation;
using GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation.Excel;
using GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation.Pdf;
using GymManagementSystem.BusinessLayer.Interfaces;

namespace GymManagementSystem.BusinessLayer.Services;

public sealed class ExportService : IExportService
{
    private readonly ExcelWriter _excelWriter;
    private readonly PdfWriter _pdfWriter;
    public ExportService()
    {
        _excelWriter = new ExcelWriter();
        _pdfWriter = new PdfWriter();
    }
    
    public Task<byte[]> ExportAsync<T>(
        IReadOnlyList<T> rows,
        IReadOnlyList<ColumnDefinition<T>> columns,
        ExportFormat format,
        string title,
        CancellationToken ct = default
    ) {
        byte[] result = format switch
        {
            ExportFormat.Excel => _excelWriter.Write(rows, columns, title),
            ExportFormat.Pdf => _pdfWriter.Write(rows, columns, title),
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };
        return Task.FromResult(result);
    }
}