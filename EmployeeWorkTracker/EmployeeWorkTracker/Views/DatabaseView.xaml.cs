using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

            var badge = FindCopyBadge(textBlock);
            if (badge is not null)
            {
                ShowCopyAnimation(badge);
            }
        }
    }

    private static Border? FindCopyBadge(TextBlock textBlock)
    {
        if (textBlock.Parent is StackPanel parent)
        {
            foreach (var child in parent.Children)
            {
                if (child is Border border && border.Name == "CopyBadge")
                {
                    return border;
                }
            }
        }
        return null;
    }

    private static void ShowCopyAnimation(Border badge)
    {
        badge.Visibility = Visibility.Visible;
        badge.Opacity = 0;

        var translateTransform = new TranslateTransform(0, 0);
        badge.RenderTransform = translateTransform;

        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150));
        var slideUp = new DoubleAnimation(0, -2, TimeSpan.FromMilliseconds(150));

        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150))
        {
            BeginTime = TimeSpan.FromMilliseconds(1200)
        };
        var slideDown = new DoubleAnimation(-2, 0, TimeSpan.FromMilliseconds(150))
        {
            BeginTime = TimeSpan.FromMilliseconds(1200)
        };

        fadeOut.Completed += (s, ev) =>
        {
            badge.Visibility = Visibility.Collapsed;
        };

        badge.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        translateTransform.BeginAnimation(TranslateTransform.YProperty, slideUp);

        badge.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        translateTransform.BeginAnimation(TranslateTransform.YProperty, slideDown);
    }
}