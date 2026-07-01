using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmployeeWorkTracker.Views;

namespace EmployeeWorkTracker.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private UserControl _currentView = new MainView();

    public MainViewModel()
    {
    }

    [RelayCommand]
    private void OpenEmployeeEdit()
    {
        var window = new EmployeeEditWindow
        {
            Owner = System.Windows.Application.Current.MainWindow
        };

        if (window.ShowDialog() == true)
        {
            CurrentView = new DatabaseView();
        }
    }

    [RelayCommand]
    private void OpenDatabase()
    {
        CurrentView = new DatabaseView();
    }

    [RelayCommand]
    private void OpenSalaryPayment()
    {
        CurrentView = new SalaryPaymentView();
    }

    [RelayCommand]
    private void OpenReports()
    {
        CurrentView = new ReportsView();
    }

    [RelayCommand]
    private void OpenMainMenu()
    {
        CurrentView = new MainView();
    }

    [RelayCommand]
    private void OpenTimeTracking()
    {
        // TODO
    }
}