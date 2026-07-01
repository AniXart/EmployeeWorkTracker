using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmployeeWorkTracker.Models;
using EmployeeWorkTracker.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EmployeeWorkTracker.ViewModels;

public partial class EmployeeEditViewModel : ObservableObject
{
    private readonly long? _employeeId;
    private readonly bool _isEditing;
    private Employee? _employee;

    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _middleName = string.Empty;
    [ObservableProperty] private string _passportNumber = string.Empty;
    [ObservableProperty] private string _phoneNumber = string.Empty;
    [ObservableProperty] private string _position = string.Empty;
    [ObservableProperty] private ImageSource? _photoPreview;
    [ObservableProperty] private byte[]? _photoBytes;
    [ObservableProperty] private ObservableCollection<Country> _countries = new();
    [ObservableProperty] private Country? _selectedCountry;
    [ObservableProperty] private string _localPhoneNumber = string.Empty;
    [ObservableProperty] private bool _isHourlyPayment;
    [ObservableProperty] private bool _isFixedPayment;
    [ObservableProperty] private decimal _hourlyRate;
    [ObservableProperty] private decimal _fixedAmount;
    [ObservableProperty] private string _comment = string.Empty;
    [ObservableProperty] private string _error = string.Empty;

    public string Title
    {
        get
        {
            var key = _isEditing ? "EditEmployee" : "NewEmployee";
            return LocalizationService.Get(key);
        }
    }

    public int CommentCounter => Comment.Length;
    public bool IsCommentAtMax => Comment.Length >= 3000;

    public event EventHandler<bool>? SaveRequested;

    public EmployeeEditViewModel(long? employeeId = null)
    {
        _employeeId = employeeId;
        _isEditing = employeeId.HasValue;

        LoadCountries();
        InitializeCountry();

        if (_isEditing)
        {
            LoadEmployee();
        }
        else
        {
            InitializeNew();
        }

        OnPropertyChanged(nameof(Title));
    }

    public void RefreshTitle()
    {
        OnPropertyChanged(nameof(Title));
    }

    private void LoadCountries()
    {
        Countries = new ObservableCollection<Country>
        {
            new() { Code = "CZ", DialCode = "+420", Flag = "🇨🇿" },
            new() { Code = "UA", DialCode = "+380", Flag = "🇺" },
            new() { Code = "RU", DialCode = "+7", Flag = "🇷🇺" },
            new() { Code = "SK", DialCode = "+421", Flag = "🇸" },
            new() { Code = "PL", DialCode = "+48", Flag = "🇵🇱" },
            new() { Code = "DE", DialCode = "+49", Flag = "🇩🇪" }
        };
    }

    private void InitializeCountry()
    {
        SelectedCountry = Countries.FirstOrDefault(c => c.Code == "CZ") ?? Countries.FirstOrDefault();
    }

    private void InitializeNew()
    {
        _employee = new Employee();
        IsHourlyPayment = true;
        IsFixedPayment = false;
    }

    private void LoadEmployee()
    {
        if (_employeeId is null) return;

        _employee = DatabaseService.GetById(_employeeId.Value);
        if (_employee is null)
        {
            Error = "Employee not found";
            return;
        }

        LastName = _employee.LastName;
        FirstName = _employee.FirstName;
        MiddleName = _employee.MiddleName;
        PassportNumber = _employee.PassportNumber;
        PhoneNumber = _employee.PhoneNumber;
        Position = _employee.Position;
        PhotoBytes = _employee.Photo;
        Comment = _employee.Comment;
        HourlyRate = _employee.HourlyRate;
        FixedAmount = _employee.FixedAmount;
        IsHourlyPayment = _employee.IsHourlyPayment;
        IsFixedPayment = !IsHourlyPayment;

        if (PhotoBytes is not null && PhotoBytes.Length > 0)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new MemoryStream(PhotoBytes);
                bitmap.EndInit();
                bitmap.Freeze();
                PhotoPreview = bitmap;
            }
            catch { }
        }

        var country = Countries.FirstOrDefault(c => PhoneNumber.StartsWith(c.DialCode));
        if (country is not null)
        {
            SelectedCountry = country;
            LocalPhoneNumber = PhoneNumber.Substring(country.DialCode.Length).Trim();
        }
    }

    partial void OnCommentChanged(string value)
    {
        OnPropertyChanged(nameof(CommentCounter));
        OnPropertyChanged(nameof(IsCommentAtMax));
    }

    [RelayCommand]
    private void LoadPhoto()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Images (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
            Title = "Select Photo"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                PhotoBytes = File.ReadAllBytes(dialog.FileName);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new MemoryStream(PhotoBytes);
                bitmap.EndInit();
                bitmap.Freeze();
                PhotoPreview = bitmap;
            }
            catch (Exception ex)
            {
                Error = $"Error loading photo: {ex.Message}";
            }
        }
    }

    [RelayCommand]
    private void DeletePhoto()
    {
        PhotoBytes = null;
        PhotoPreview = null;
    }

    [RelayCommand]
    private void SetHourlyPayment()
    {
        IsHourlyPayment = true;
        IsFixedPayment = false;
    }

    [RelayCommand]
    private void SetFixedPayment()
    {
        IsHourlyPayment = false;
        IsFixedPayment = true;
    }

    [RelayCommand]
    private void Save()
    {
        Error = string.Empty;

        if (string.IsNullOrWhiteSpace(LastName))
        {
            Error = LocalizationService.Get("LastName") + " is required";
            return;
        }

        if (string.IsNullOrWhiteSpace(FirstName))
        {
            Error = LocalizationService.Get("FirstName") + " is required";
            return;
        }

        if (string.IsNullOrWhiteSpace(PassportNumber))
        {
            Error = LocalizationService.Get("PassportNumber") + " is required";
            return;
        }

        if (string.IsNullOrWhiteSpace(LocalPhoneNumber))
        {
            Error = LocalizationService.Get("Phone") + " is required";
            return;
        }

        if (string.IsNullOrWhiteSpace(Position))
        {
            Error = LocalizationService.Get("Position") + " is required";
            return;
        }

        if (_employee is null) return;

        _employee.LastName = LastName;
        _employee.FirstName = FirstName;
        _employee.MiddleName = MiddleName;
        _employee.PassportNumber = PassportNumber;
        _employee.Position = Position;
        _employee.Photo = PhotoBytes;
        _employee.Comment = Comment;
        _employee.HourlyRate = HourlyRate;
        _employee.FixedAmount = FixedAmount;
        _employee.IsHourlyPayment = IsHourlyPayment;

        var fullPhone = SelectedCountry?.DialCode + LocalPhoneNumber;
        _employee.PhoneNumber = fullPhone;

        if (_isEditing)
        {
            DatabaseService.UpdateEmployee(_employee);
        }
        else
        {
            DatabaseService.AddEmployee(_employee);
        }

        SaveRequested?.Invoke(this, true);
    }

    [RelayCommand]
    private void Cancel()
    {
        SaveRequested?.Invoke(this, false);
    }
}

public class Country
{
    public string Code { get; set; } = string.Empty;
    public string DialCode { get; set; } = string.Empty;
    public string Flag { get; set; } = string.Empty;

    public override string ToString() => $"{Flag} {Code} {DialCode}";
}