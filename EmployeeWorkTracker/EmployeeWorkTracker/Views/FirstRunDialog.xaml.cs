using System.Windows;
using System.Windows.Input;
using EmployeeWorkTracker.Services;

namespace EmployeeWorkTracker.Views;

public partial class FirstRunDialog : Window
{
    public FirstRunDialog()
    {
        InitializeComponent();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2) return;
        DragMove();
    }

    private void ContinueButton_Click(object sender, RoutedEventArgs e)
    {
        AppLanguage selectedLanguage = AppLanguage.English;

        if (RadioRussian.IsChecked == true) selectedLanguage = AppLanguage.Russian;
        else if (RadioUkrainian.IsChecked == true) selectedLanguage = AppLanguage.Ukrainian;
        else if (RadioCzech.IsChecked == true) selectedLanguage = AppLanguage.Czech;

        LocalizationService.SetLanguage(selectedLanguage);
        DialogResult = true;
        Close();
    }
}