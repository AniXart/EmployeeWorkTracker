namespace EmployeeWorkTracker.Services;

public static class AuthService
{
    public static AuthSession Session { get; } = new();

    public static void Logout()
    {
        Session.CurrentUser = null;
    }
}

public class AuthSession
{
    public Models.UserAccount? CurrentUser { get; set; }
    public bool IsAuthenticated => CurrentUser is not null;
    public bool IsAdmin => true; // Все пользователи имеют полный доступ
}