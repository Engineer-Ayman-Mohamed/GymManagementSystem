using System.Net;
using Shouldly;

namespace GymManagementSystem.PresentationLayer.Testing.integrations;

public class MemberIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MemberIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task I1_MemberIndex_ShouldReturn200WithHtml()
    {
        var response = await _client.GetAsync("/Members");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task I2_ExportExcel_ShouldReturnSpreadsheet()
    {
        var response = await _client.GetAsync("/Members/ExportExcel");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var contentType = response.Content.Headers.ContentType?.MediaType;
        contentType.ShouldNotBeNull();
        contentType.ShouldContain("spreadsheet");

        var bytes = await response.Content.ReadAsByteArrayAsync();
        bytes.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task I3_ExportPdf_ShouldReturnValidPdf()
    {
        var response = await _client.GetAsync("/Members/ExportPdf");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var contentType = response.Content.Headers.ContentType?.MediaType;
        contentType.ShouldNotBeNull();
        contentType.ShouldContain("pdf");

        var bytes = await response.Content.ReadAsByteArrayAsync();
        bytes.Length.ShouldBeGreaterThan(0);

        var header = System.Text.Encoding.ASCII.GetString(bytes, 0, 5);
        header.ShouldBe("%PDF-");
    }
}
