namespace GymManagementSystem.BusinessLayer.Exceptions;

public abstract class AppException : Exception
{
    public int StatusCode { get; }
    public Dictionary<string, string[]>? Details { get; }

    protected AppException(string message, int statusCode, Dictionary<string, string[]>? details = null)
        : base(message)
    {
        StatusCode = statusCode;
        Details = details;
    }
}
