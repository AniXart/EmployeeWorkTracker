using EmployeeWorkTracker.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace EmployeeWorkTracker.Services;

public static class DatabaseService
{
    private static readonly string AppDataFolder = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "AppData");

    private static readonly string EmployeesFilePath = Path.Combine(AppDataFolder, "employees.json");
    private static readonly string PaymentsFilePath = Path.Combine(AppDataFolder, "payments.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static ObservableCollection<Employee> Employees { get; } = new();
    public static ObservableCollection<SalaryPayment> Payments { get; } = new();

    static DatabaseService()
    {
        LoadFromDisk();
    }

    #region Employees

    public static void AddEmployee(Employee employee)
    {
        employee.Id = Employees.Count > 0 ? Employees.Max(e => e.Id) + 1 : 1;
        Employees.Add(employee);
        SaveEmployeesToDisk();
    }

    public static Employee? GetById(long id)
    {
        return Employees.FirstOrDefault(e => e.Id == id);
    }

    public static Employee? GetByPassport(string passport)
    {
        return Employees.FirstOrDefault(e =>
            e.PassportNumber.Equals(passport, StringComparison.OrdinalIgnoreCase));
    }

    public static void RemoveEmployee(long id)
    {
        var emp = Employees.FirstOrDefault(e => e.Id == id);
        if (emp is not null)
        {
            Employees.Remove(emp);
            SaveEmployeesToDisk();
        }
    }

    public static void UpdateEmployee(Employee updated)
    {
        var existing = Employees.FirstOrDefault(e => e.Id == updated.Id);
        if (existing is not null)
        {
            var idx = Employees.IndexOf(existing);
            Employees[idx] = updated;
            SaveEmployeesToDisk();
        }
    }

    private static void SaveEmployeesToDisk()
    {
        try
        {
            if (!Directory.Exists(AppDataFolder))
                Directory.CreateDirectory(AppDataFolder);

            var json = JsonSerializer.Serialize(Employees, JsonOptions);
            File.WriteAllText(EmployeesFilePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка сохранения сотрудников: {ex.Message}");
        }
    }

    #endregion

    #region Payments

    public static void AddPayment(SalaryPayment payment)
    {
        payment.Id = Payments.Count > 0 ? Payments.Max(p => p.Id) + 1 : 1;
        Payments.Add(payment);
        SavePaymentsToDisk();
    }

    public static ObservableCollection<SalaryPayment> GetPaymentsByEmployee(long employeeId)
    {
        return new ObservableCollection<SalaryPayment>(
            Payments.Where(p => p.EmployeeId == employeeId).OrderByDescending(p => p.CreatedAt));
    }

    public static void RemovePayment(long id)
    {
        var payment = Payments.FirstOrDefault(p => p.Id == id);
        if (payment is not null)
        {
            Payments.Remove(payment);
            SavePaymentsToDisk();
        }
    }

    private static void SavePaymentsToDisk()
    {
        try
        {
            if (!Directory.Exists(AppDataFolder))
                Directory.CreateDirectory(AppDataFolder);

            var json = JsonSerializer.Serialize(Payments, JsonOptions);
            File.WriteAllText(PaymentsFilePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка сохранения начислений: {ex.Message}");
        }
    }

    #endregion

    private static void LoadFromDisk()
    {
        try
        {
            if (File.Exists(EmployeesFilePath))
            {
                var json = File.ReadAllText(EmployeesFilePath);
                var employees = JsonSerializer.Deserialize<List<Employee>>(json, JsonOptions);
                if (employees is not null)
                {
                    foreach (var emp in employees)
                        Employees.Add(emp);
                }
            }

            if (File.Exists(PaymentsFilePath))
            {
                var json = File.ReadAllText(PaymentsFilePath);
                var payments = JsonSerializer.Deserialize<List<SalaryPayment>>(json, JsonOptions);
                if (payments is not null)
                {
                    foreach (var payment in payments)
                        Payments.Add(payment);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
        }
    }
}