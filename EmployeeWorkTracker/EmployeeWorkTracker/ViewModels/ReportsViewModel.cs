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

    public ReportsViewModel()
    {
        RefreshPayments();
    }

    public void RefreshPayments()
    {
        Payments = new ObservableCollection<SalaryPayment>(
            DatabaseService.Payments.OrderByDescending(p => p.CreatedAt));
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
    }

    [RelayCommand]
    private void DeletePayment(SalaryPayment? payment)
    {
        if (payment is null) return;

        DatabaseService.RemovePayment(payment.Id);
        RefreshPayments();
    }
}