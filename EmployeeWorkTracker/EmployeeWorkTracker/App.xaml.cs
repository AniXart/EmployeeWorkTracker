using System.Windows;
using EmployeeWorkTracker.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeWorkTracker;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // Регистрация ViewModel
        services.AddSingleton<MainViewModel>();

        // Регистрация MainWindow
        services.AddSingleton<MainWindow>();

        Services = services.BuildServiceProvider();
    }
}