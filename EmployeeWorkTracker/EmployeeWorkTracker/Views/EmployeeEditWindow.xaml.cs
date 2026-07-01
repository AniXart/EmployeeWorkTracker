using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EmployeeWorkTracker.ViewModels;

namespace EmployeeWorkTracker.Views;

public partial class EmployeeEditWindow : Window
{
    private readonly EmployeeEditViewModel _viewModel;

    public EmployeeEditWindow()
    {
        InitializeComponent();
        _viewModel = new EmployeeEditViewModel();
        DataContext = _viewModel;
    }

    public EmployeeEditWindow(long employeeId)
    {
        InitializeComponent();
        _viewModel = new EmployeeEditViewModel(employeeId);
        DataContext = _viewModel;
        _viewModel.LoadEmployeeData();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2) return;
        DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }
        else
        {
            WindowState = WindowState.Maximized;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

    private void ComboBoxBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.TemplatedParent is ComboBox combo)
        {
            combo.IsDropDownOpen = !combo.IsDropDownOpen;
            e.Handled = true;
        }
    }
}