using EmployeeWorkTracker.Models;
using EmployeeWorkTracker.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace EmployeeWorkTracker.Views;

public partial class ReportDetailView : Window
{
    public ReportDetailView(SalaryPayment payment)
    {
        InitializeComponent();
        DataContext = new ReportDetailViewModel(payment);
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2) return;
        DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}