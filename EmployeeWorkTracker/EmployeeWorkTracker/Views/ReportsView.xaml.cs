using System.Windows.Controls;
using System.Windows.Input;
using EmployeeWorkTracker.Models;
using EmployeeWorkTracker.ViewModels;

namespace EmployeeWorkTracker.Views;

public partial class ReportsView : UserControl
{
    public ReportsView()
    {
        InitializeComponent();
    }

    private void PaymentCard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.Tag is SalaryPayment payment)
        {
            if (DataContext is ReportsViewModel vm)
            {
                vm.ViewPaymentCommand.Execute(payment);
            }
        }
    }
}