namespace EmployeeWorkTracker.Models;

public enum UserRole
{
    Admin,
    User
}

public sealed class UserAccount
{
    public string Username { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Admin;
}