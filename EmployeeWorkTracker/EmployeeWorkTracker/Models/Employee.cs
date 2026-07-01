using System.Text.Json.Serialization;

namespace EmployeeWorkTracker.Models;

public sealed class Employee
{
    public long Id { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public byte[]? Photo { get; set; }
    public string Comment { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public decimal FixedAmount { get; set; }
    public bool IsHourlyPayment { get; set; } = true;

    public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
}