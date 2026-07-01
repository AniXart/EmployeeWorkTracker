using CommunityToolkit.Mvvm.ComponentModel;
using EmployeeWorkTracker.Services;

namespace EmployeeWorkTracker.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] private AppLanguage _selectedLanguage;

    public SettingsViewModel()
    {
        SelectedLanguage = LocalizationService.CurrentLanguage;
    }

    partial void OnSelectedLanguageChanged(AppLanguage value)
    {
        LocalizationService.SetLanguage(value);
    }
}