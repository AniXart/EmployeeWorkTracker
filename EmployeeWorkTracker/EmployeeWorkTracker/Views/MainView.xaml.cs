using System.Windows.Controls;
using System.Windows.Input;
using EmployeeWorkTracker.ViewModels;

namespace EmployeeWorkTracker.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void EmployeesCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.OpenEmployeeEditCommand.Execute(null);
    }

    private void DatabaseCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.OpenDatabaseCommand.Execute(null);
    }

    private void SalaryCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.OpenSalaryPaymentCommand.Execute(null);
    }

    private void ReportsCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.OpenReportsCommand.Execute(null);
    }
}