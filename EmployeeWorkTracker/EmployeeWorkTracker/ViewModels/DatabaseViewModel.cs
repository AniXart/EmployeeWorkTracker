using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmployeeWorkTracker.Models;
using EmployeeWorkTracker.Services;
using EmployeeWorkTracker.Views;

namespace EmployeeWorkTracker.ViewModels;

public partial class DatabaseViewModel : ObservableObject
{
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Employee> _filteredEmployees = [];

    [ObservableProperty]
    private bool _showCopyNotification;

    public DatabaseViewModel()
    {
        RefreshList();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    public void RefreshList()
    {
        FilteredEmployees = [.. DatabaseService.Employees];
    }

    private void ApplyFilter()
    {
        var q = SearchText.Trim().ToUpperInvariant();
        var filtered = string.IsNullOrEmpty(q)
            ? [.. DatabaseService.Employees]
            : DatabaseService.Employees
                .Where(e => e.PassportNumber.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                            e.FullName.Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList();

        FilteredEmployees = [.. filtered];
    }

    [RelayCommand]
    private void AddEmployee()
    {
        var window = new EmployeeEditWindow
        {
            Owner = Application.Current.MainWindow
        };

        if (window.ShowDialog() == true)
        {
            RefreshList();
        }
    }

    [RelayCommand]
    private void EditEmployee(Employee? emp)
    {
        if (emp is null) return;

        var window = new EmployeeEditWindow(emp.Id)
        {
            Owner = Application.Current.MainWindow
        };

        if (window.ShowDialog() == true)
        {
            RefreshList();
        }
    }

    [RelayCommand]
    private void DeleteEmployee(Employee? emp)
    {
        if (emp is null) return;

        DatabaseService.RemoveEmployee(emp.Id);
        RefreshList();
    }

    [RelayCommand]
    private void CopyPassport(Employee? emp)
    {
        if (emp is null) return;

        Clipboard.SetText(emp.PassportNumber);

        ShowCopyNotification = true;

        Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            await Task.Delay(2000);
            ShowCopyNotification = false;
        });
    }
}