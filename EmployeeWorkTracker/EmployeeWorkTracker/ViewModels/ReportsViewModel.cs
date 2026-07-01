using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmployeeWorkTracker.Models;
using EmployeeWorkTracker.Services;
using EmployeeWorkTracker.Views;

namespace EmployeeWorkTracker.ViewModels;

public partial class ReportsViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<SalaryPayment> _payments = new();
    [ObservableProperty] private ObservableCollection<SalaryPayment> _filteredPayments = new();
    [ObservableProperty] private string _passportSearch = string.Empty;
    [ObservableProperty] private string _dateSearch = string.Empty;

    public ReportsViewModel()
    {
        RefreshPayments();
    }

    public void RefreshPayments()
    {
        Payments = new ObservableCollection<SalaryPayment>(
            DatabaseService.Payments.OrderByDescending(p => p.CreatedAt));
        ApplyFilter();
    }

    partial void OnPassportSearchChanged(string value) => ApplyFilter();
    partial void OnDateSearchChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var passportQuery = PassportSearch.Trim().ToUpperInvariant();
        var dateQuery = DateSearch.Trim();

        var filtered = Payments.Where(p =>
        {
            var passportMatch = string.IsNullOrEmpty(passportQuery) ||
                               p.PassportNumber.ToUpperInvariant().Contains(passportQuery);

            var dateMatch = string.IsNullOrEmpty(dateQuery) ||
                           p.CreatedAt.ToString("dd.MM.yyyy").Contains(dateQuery);

            return passportMatch && dateMatch;
        }).ToList();

        FilteredPayments = new ObservableCollection<SalaryPayment>(filtered);
    }

    [RelayCommand]
    private void ViewPayment(SalaryPayment? payment)
    {
        if (payment is null) return;

        var window = new ReportDetailView(payment)
        {
            Owner = Application.Current.MainWindow
        };
        window.ShowDialog();

        RefreshPayments();
    }

    [RelayCommand]
    private void DeletePayment(SalaryPayment? payment)
    {
        if (payment is null) return;

        DatabaseService.RemovePayment(payment.Id);
        RefreshPayments();
    }

    [RelayCommand]
    private void ClearSearch()
    {
        PassportSearch = string.Empty;
        DateSearch = string.Empty;
    }
}