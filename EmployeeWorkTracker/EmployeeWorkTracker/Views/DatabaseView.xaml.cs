using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EmployeeWorkTracker.Models;
using EmployeeWorkTracker.ViewModels;

namespace EmployeeWorkTracker.Views;

public partial class DatabaseView : UserControl
{
    public DatabaseView()
    {
        InitializeComponent();
    }

    private void PassportText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBlock textBlock && textBlock.Tag is Employee employee)
        {
            if (DataContext is DatabaseViewModel viewModel)
            {
                viewModel.CopyPassportCommand.Execute(employee);
            }
        }
    }
}