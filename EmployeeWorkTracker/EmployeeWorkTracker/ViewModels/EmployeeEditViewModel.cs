using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmployeeWorkTracker.Models;
using EmployeeWorkTracker.Services;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace EmployeeWorkTracker.ViewModels;

public sealed partial class EmployeeEditViewModel : ObservableObject
{
    private const int MaxCommentLength = 3000;
    private static readonly Regex PassportRegex = new(@"^[A-Z]{2}\d{6}$", RegexOptions.Compiled);

    public long? EmployeeId { get; }
    public bool IsEditMode => EmployeeId.HasValue;
    public string Title => IsEditMode ? "Редактирование сотрудника" : "Новый сотрудник";

    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _middleName = string.Empty;
    [ObservableProperty] private string _passportNumber = string.Empty;
    [ObservableProperty] private string _phoneNumber = string.Empty;
    [ObservableProperty] private string _position = string.Empty;
    [ObservableProperty] private string _paymentType = "Hourly";
    [ObservableProperty] private decimal _hourlyRate;
    [ObservableProperty] private decimal _fixedAmount;
    [ObservableProperty] private string _comment = string.Empty;
    [ObservableProperty] private byte[]? _photo;
    [ObservableProperty] private BitmapSource? _photoPreview;
    [ObservableProperty] private Country? _selectedCountry;
    [ObservableProperty] private string _localPhoneNumber = string.Empty;
    [ObservableProperty] private string _error = string.Empty;

    public IReadOnlyList<Country> Countries { get; } = CountryService.GetCountries();

    public string CommentCounter => $"{Comment?.Length ?? 0}/{MaxCommentLength}";
    public bool IsCommentAtMax => Comment?.Length >= MaxCommentLength;

    public EmployeeEditViewModel() : this(null)
    {
    }

    public EmployeeEditViewModel(long? employeeId)
    {
        EmployeeId = employeeId;
        SelectedCountry = Countries.FirstOrDefault(c => c.Code == "CZ") ?? Countries[0];
    }

    public bool IsHourlyPayment => PaymentType == "Hourly";
    public bool IsFixedPayment => PaymentType == "Fixed";

    partial void OnSelectedCountryChanged(Country? value) => UpdateFullPhoneNumber();
    partial void OnLocalPhoneNumberChanged(string value) => UpdateFullPhoneNumber();

    // ✅ Обновляем счётчик комментария
    partial void OnCommentChanged(string value)
    {
        OnPropertyChanged(nameof(CommentCounter));
        OnPropertyChanged(nameof(IsCommentAtMax));
    }

    private void UpdateFullPhoneNumber()
    {
        if (SelectedCountry is not null)
        {
            var digits = new string(LocalPhoneNumber.Where(char.IsDigit).ToArray());
            PhoneNumber = string.IsNullOrEmpty(digits) ? string.Empty : $"{SelectedCountry.DialCode}{digits}";
        }
        else
        {
            PhoneNumber = string.Empty;
        }
    }

    public void LoadEmployeeData()
    {
        if (!IsEditMode || EmployeeId is null) return;

        var existing = DatabaseService.GetById(EmployeeId.Value);
        if (existing is null) return;

        LastName = existing.LastName;
        FirstName = existing.FirstName;
        MiddleName = existing.MiddleName;
        PassportNumber = existing.PassportNumber;
        PhoneNumber = existing.PhoneNumber;
        Position = existing.Position;
        PaymentType = existing.PaymentType;
        HourlyRate = existing.HourlyRate;
        FixedAmount = existing.FixedAmount;
        Comment = existing.Comment;
        Photo = existing.Photo;

        ParsePhoneNumber(existing.PhoneNumber);

        if (Photo is not null && Photo.Length > 0)
        {
            PhotoPreview = LoadPhotoFromBytes(Photo);
        }
        else
        {
            PhotoPreview = null;
        }

        OnPropertyChanged(nameof(IsHourlyPayment));
        OnPropertyChanged(nameof(IsFixedPayment));
        OnPropertyChanged(nameof(CommentCounter));
    }

    private void ParsePhoneNumber(string fullNumber)
    {
        if (string.IsNullOrEmpty(fullNumber)) return;

        foreach (var country in Countries)
        {
            if (fullNumber.StartsWith(country.DialCode))
            {
                SelectedCountry = country;
                LocalPhoneNumber = fullNumber.Substring(country.DialCode.Length);
                return;
            }
        }
        LocalPhoneNumber = fullNumber.TrimStart('+');
    }

    [RelayCommand]
    private void LoadPhoto()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
            Title = "Выберите фотографию"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                var bytes = File.ReadAllBytes(dialog.FileName);
                Photo = bytes;
                PhotoPreview = LoadPhotoFromBytes(bytes);
            }
            catch (Exception ex)
            {
                Error = $"Ошибка загрузки фото: {ex.Message}";
            }
        }
    }

    [RelayCommand]
    private void DeletePhoto()
    {
        Photo = null;
        PhotoPreview = null;
    }

    [RelayCommand]
    private void SetHourlyPayment()
    {
        PaymentType = "Hourly";
        OnPropertyChanged(nameof(IsHourlyPayment));
        OnPropertyChanged(nameof(IsFixedPayment));
    }

    [RelayCommand]
    private void SetFixedPayment()
    {
        PaymentType = "Fixed";
        OnPropertyChanged(nameof(IsHourlyPayment));
        OnPropertyChanged(nameof(IsFixedPayment));
    }

    [RelayCommand]
    private void Save()
    {
        Error = Validate();
        if (!string.IsNullOrEmpty(Error)) return;

        var employee = new Employee
        {
            Id = EmployeeId ?? 0,
            LastName = LastName.Trim(),
            FirstName = FirstName.Trim(),
            MiddleName = MiddleName.Trim(),
            PassportNumber = PassportNumber.Trim().ToUpperInvariant(),
            PhoneNumber = PhoneNumber,
            Position = Position.Trim(),
            PaymentType = PaymentType,
            HourlyRate = PaymentType == "Hourly" ? HourlyRate : 0,
            FixedAmount = PaymentType == "Fixed" ? FixedAmount : 0,
            Comment = Comment,
            Photo = Photo
        };

        if (IsEditMode)
        {
            DatabaseService.UpdateEmployee(employee);
        }
        else
        {
            DatabaseService.AddEmployee(employee);
        }

        if (Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this) is Window window)
        {
            window.DialogResult = true;
            window.Close();
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        if (Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this) is Window window)
        {
            window.DialogResult = false;
            window.Close();
        }
    }

    private string Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(LastName))
            errors.Add("Фамилия обязательна.");
        if (string.IsNullOrWhiteSpace(FirstName))
            errors.Add("Имя обязательно.");
        if (string.IsNullOrWhiteSpace(Position))
            errors.Add("Должность обязательна.");

        var passport = PassportNumber.Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(passport))
            errors.Add("Номер паспорта обязателен.");
        else if (!PassportRegex.IsMatch(passport))
            errors.Add("Неверный формат паспорта. Ожидается: 2 заглавные латинские буквы + 6 цифр (например, AB123456).");

        if (string.IsNullOrEmpty(PhoneNumber))
            errors.Add("Номер телефона обязателен.");
        else if (!PhoneNumber.StartsWith("+"))
            errors.Add("Номер телефона должен начинаться с +.");
        else if (PhoneNumber.Length < 8)
            errors.Add("Номер телефона слишком короткий.");

        if (PaymentType == "Hourly" && HourlyRate <= 0)
            errors.Add("Почасовая ставка должна быть больше нуля.");
        else if (PaymentType == "Fixed" && FixedAmount <= 0)
            errors.Add("Фиксированная сумма должна быть больше нуля.");

        if (Comment.Length > MaxCommentLength)
            errors.Add($"Комментарий не должен превышать {MaxCommentLength} символов");

        return errors.Count > 0 ? string.Join(Environment.NewLine, errors) : string.Empty;
    }

    private static BitmapSource? LoadPhotoFromBytes(byte[] bytes)
    {
        if (bytes is null || bytes.Length == 0)
            return null;

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = new MemoryStream(bytes);
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        catch
        {
            return null;
        }
    }
}