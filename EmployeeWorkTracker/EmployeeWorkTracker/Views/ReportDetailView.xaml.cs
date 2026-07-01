using EmployeeWorkTracker.Models;
using EmployeeWorkTracker.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

    private void DeletePhotoButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is ImageSource imageSource)
        {
            if (DataContext is ReportDetailViewModel viewModel)
            {
                var index = viewModel.ReceiptPhotoSources.IndexOf(imageSource);
                if (index >= 0)
                {
                    viewModel.DeleteReceiptPhoto(index);
                }
            }
        }
    }
}