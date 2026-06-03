namespace GymManagementSystem.BusinessLayer.Exceptions;

public class ValidationException : AppException
{
    public ValidationException(string message, Dictionary<string, string[]>? errors = null)
        : base(message, 400, errors)
    {
    }
}
