namespace GymManagementSystem.PresentationLayer.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public int StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ExceptionDetails { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public bool ShowExceptionDetails => !string.IsNullOrEmpty(ExceptionDetails);
}
