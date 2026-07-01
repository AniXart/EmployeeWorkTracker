using System.Windows;
using System.Windows.Input;
using EmployeeWorkTracker.Services;
using EmployeeWorkTracker.Views;

namespace EmployeeWorkTracker;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (LocalizationService.IsFirstRun)
        {
            var dialog = new FirstRunDialog
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var result = dialog.ShowDialog();

            if (result != true)
            {
                Close();
            }
        }
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
}