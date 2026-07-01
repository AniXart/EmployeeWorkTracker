using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EmployeeWorkTracker.Services;
using EmployeeWorkTracker.ViewModels;

namespace EmployeeWorkTracker.Views;

public partial class EmployeeEditWindow : Window
{
    public EmployeeEditWindow(long? employeeId = null)
    {
        InitializeComponent();

        var viewModel = new EmployeeEditViewModel(employeeId);
        DataContext = viewModel;

        viewModel.SaveRequested += (sender, result) =>
        {
            DialogResult = result;
            Close();
        };
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2) return;
        DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

    private void ComboBoxBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.TemplatedParent is ComboBox comboBox)
        {
            comboBox.IsDropDownOpen = !comboBox.IsDropDownOpen;
            e.Handled = true;
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        if (DataContext is EmployeeEditViewModel vm)
        {
            vm.RefreshTitle();
        }
    }
}