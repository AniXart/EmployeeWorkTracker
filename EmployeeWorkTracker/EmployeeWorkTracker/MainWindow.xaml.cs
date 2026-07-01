using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EmployeeWorkTracker.ViewModels;
using EmployeeWorkTracker.Views;

namespace EmployeeWorkTracker;

public partial class MainWindow : Window
{
    private Button? _activeNavButton;
    private EmployeeEditWindow? _editWindow;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        _activeNavButton = NavHome;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2) return;
        DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

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

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void SetActiveNavButton(Button button)
    {
        if (_activeNavButton == button) return;

        if (_activeNavButton is not null)
        {
            _activeNavButton.Style = FindResource("NavButton") as Style;
        }

        button.Style = FindResource("NavButtonActive") as Style;
        _activeNavButton = button;
    }

    private void CloseEditWindowIfOpen()
    {
        if (_editWindow is not null)
        {
            _editWindow.Close();
            _editWindow = null;
        }
    }

    private void MainMenu_Click(object sender, RoutedEventArgs e)
    {
        CloseEditWindowIfOpen();
        SetActiveNavButton(NavHome);
        if (DataContext is MainViewModel vm)
            vm.OpenMainMenuCommand.Execute(null);
    }

    private void Employees_Click(object sender, RoutedEventArgs e)
    {
        CloseEditWindowIfOpen();
        SetActiveNavButton(NavEmployees);
        if (DataContext is MainViewModel vm)
            vm.OpenEmployeeEditCommand.Execute(null);
    }

    private void Database_Click(object sender, RoutedEventArgs e)
    {
        CloseEditWindowIfOpen();
        SetActiveNavButton(NavDatabase);
        if (DataContext is MainViewModel vm)
            vm.OpenDatabaseCommand.Execute(null);
    }

    private void Salary_Click(object sender, RoutedEventArgs e)
    {
        CloseEditWindowIfOpen();
        SetActiveNavButton(NavTime);
        if (DataContext is MainViewModel vm)
            vm.OpenSalaryPaymentCommand.Execute(null);
    }

    // ✅ Добавлен обработчик для кнопки "Отчёты"
    private void Reports_Click(object sender, RoutedEventArgs e)
    {
        CloseEditWindowIfOpen();
        SetActiveNavButton(NavReports);
        if (DataContext is MainViewModel vm)
            vm.OpenReportsCommand.Execute(null);
    }
}