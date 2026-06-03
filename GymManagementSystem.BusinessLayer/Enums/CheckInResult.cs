namespace GymManagementSystem.BusinessLayer.Enums;

public enum CheckInResult
{
    Success,
    InvalidFormat,
    InvalidSignature,
    NotFound,
    AlreadyAttended,
    SessionNotToday
}