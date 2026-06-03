using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation.Pdf;

public sealed class PdfWriter
{
    public PdfWriter() { QuestPDF.Settings.License = LicenseType.Community; }

    public byte[] Write<T>(
        IReadOnlyList<T> rows,
        IReadOnlyList<ColumnDefinition<T>> columns,
        string title)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);

                page.Header().Text(title).SemiBold().FontSize(16);

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(def =>
                    {
                        foreach (var _ in columns)
                            def.RelativeColumn();
                    });

                    foreach (var col in columns)
                    {
                        table.Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(2)
                            .Text(col.Header).SemiBold().FontSize(10);
                    }

                    foreach (var row in rows)
                    {
                        foreach (var col in columns)
                        {
                            var value = col.ValueSelector(row);
                            table.Cell()
                                .Padding(2)
                                .Text(value?.ToString() ?? "").FontSize(10);
                        }
                    }
                });
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }
}