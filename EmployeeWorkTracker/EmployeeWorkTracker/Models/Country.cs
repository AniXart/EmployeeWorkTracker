namespace EmployeeWorkTracker.Models;

public sealed record Country(
    string Name,
    string Code,
    string DialCode,
    string FlagType,
    string PrimaryColor,
    string SecondaryColor,
    string TertiaryColor = "")
{
    public string DisplayName => $"{Code} ({DialCode})";
}