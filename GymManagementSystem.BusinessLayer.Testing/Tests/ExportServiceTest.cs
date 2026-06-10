using ClosedXML.Excel;
using GymManagementSystem.BusinessLayer.Enums;
using GymManagementSystem.BusinessLayer.Export.ExportFormatsImplementation;
using GymManagementSystem.BusinessLayer.Services;
using Shouldly;

namespace GymManagementSystem.BusinessLayer.Testing.Tests;

public class ExportServiceTest
{
    private record TestRow(int Id, string Name);

    private static readonly IReadOnlyList<TestRow> SampleRows =
    [
        new TestRow(1, "Alice"),
        new TestRow(2, "Bob"),
        new TestRow(3, "Charlie")
    ];

    private static readonly IReadOnlyList<ColumnDefinition<TestRow>> SampleColumns =
    [
        new ColumnDefinition<TestRow>("Id", r => r.Id),
        new ColumnDefinition<TestRow>("Full Name", r => r.Name)
    ];
    
    [Fact]
    public async Task U1_ExportExcelWithData_ShouldReturnNonEmptyBytes()
    {
        var service = new ExportService();

        var result = await service
            .ExportAsync(
            SampleRows,
            SampleColumns,
            ExportFormat.Excel,
            "Test Report"
        );

        result.ShouldNotBeNull();
        result.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task U2_ExportPdfWithData_ShouldReturnValidPdf()
    {
        var service = new ExportService();

        var result = await service
            .ExportAsync(
            SampleRows,
            SampleColumns,
            ExportFormat.Pdf,
            "Test Report"
        );

        result.ShouldNotBeNull();
        result.Length.ShouldBeGreaterThan(0);

        var header = System.Text.Encoding.ASCII.GetString(result, 0, 5);
        header.ShouldBe("%PDF-");
    }

    [Fact]
    public async Task U3_ExportExcelEmptyList_ShouldReturnBytesWithoutCrashing()
    {
        var service = new ExportService();
        var emptyRows = new List<TestRow>();

        var result = await service
            .ExportAsync(
            emptyRows,
            SampleColumns,
            ExportFormat.Excel,
            "Empty Report"
        );

        result.ShouldNotBeNull();
        result.Length.ShouldBeGreaterThan(0); 
    }

    [Fact]
    public async Task U3_ExportPdfEmptyList_ShouldReturnBytesWithoutCrashing()
    {
        var service = new ExportService();
        var emptyRows = new List<TestRow>();

        var result = await service.ExportAsync(
            emptyRows, SampleColumns, ExportFormat.Pdf, "Empty Report");

        result.ShouldNotBeNull();
        result.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task U4_ExportExcel_ColumnMapping_ShouldReflectValues()
    {
        var service = new ExportService();
        var rows = new List<TestRow> { new TestRow(42, "TestUser") };
        var columns = new List<ColumnDefinition<TestRow>>
        {
            new("FullName", r => r.Name)
        };

        var bytes = await service
            .ExportAsync(
            rows,
            columns,
            ExportFormat.Excel,
            "Mapping Test"
        );

        using var stream = new MemoryStream(bytes);
        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheet(1);

        sheet.Cell(2, 1).GetString().ShouldBe("TestUser");
        sheet.Cell(1, 1).GetString().ShouldBe("FullName");
    }
}