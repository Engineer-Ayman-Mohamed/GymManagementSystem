using ClosedXML.Excel;

namespace GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation.Excel;
/// <summary>
/// "Maps rows + column definitions into a structured Excel sheet and returns it as a downloadable byte array."
/// </summary>
public sealed class ExcelWriter
{
    public byte[] Write<T>(
        IReadOnlyList<T> rows,
        IReadOnlyList<ColumnDefinition<T>> columns,
        string title
    )
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet(title);
        
        for (int i = 0; i < columns.Count; i++)
            sheet.Cell(1, i + 1).Value = columns[i].Header;
        
        for (int i = 0; i < rows.Count; i++)
        for (int j = 0; j < columns.Count; j++)
            sheet.Cell(i + 2, j + 1).Value = columns[j].ValueSelector(rows[i])?.ToString() ?? "";
        
        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return stream.ToArray();
    }
}