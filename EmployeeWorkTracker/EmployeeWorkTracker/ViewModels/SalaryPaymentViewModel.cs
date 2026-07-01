using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmployeeWorkTracker.Models;
using EmployeeWorkTracker.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace EmployeeWorkTracker.ViewModels;

public sealed partial class SalaryPaymentViewModel : ObservableObject
{
    private const int MaxCommentLength = 3000;
    private const int MinYear = 2004;
    private const int MaxYear = 5000;

    [ObservableProperty] private string _passportSearch = string.Empty;
    [ObservableProperty] private string _searchError = string.Empty;
    [ObservableProperty] private Employee? _selectedEmployee;
    [ObservableProperty] private DateOnly _dateFrom = DateOnly.FromDateTime(DateTime.Today);
    [ObservableProperty] private DateOnly _dateTo = DateOnly.FromDateTime(DateTime.Today);
    [ObservableProperty] private string _dateFromText = string.Empty;
    [ObservableProperty] private string _dateToText = string.Empty;
    [ObservableProperty] private string _city = string.Empty;
    [ObservableProperty] private string _address = string.Empty;
    [ObservableProperty] private string _workObject = string.Empty;
    [ObservableProperty] private ObservableCollection<WorkAddress> _workAddresses = new();
    [ObservableProperty] private byte[]? _receiptPhoto;
    [ObservableProperty] private BitmapSource? _receiptPhotoPreview;
    [ObservableProperty] private string _comment = string.Empty;
    [ObservableProperty] private string _error = string.Empty;
    [ObservableProperty] private bool _employeeFound;

    public SalaryPaymentViewModel()
    {
        DateFromText = DateFrom.ToString("dd.MM.yyyy");
        DateToText = DateTo.ToString("dd.MM.yyyy");
    }

    public string CommentCounter => $"{Comment?.Length ?? 0}/{MaxCommentLength}";
    public bool IsCommentAtMax => Comment?.Length >= MaxCommentLength;

    partial void OnCommentChanged(string value)
    {
        OnPropertyChanged(nameof(CommentCounter));
        OnPropertyChanged(nameof(IsCommentAtMax));
    }

    partial void OnDateFromTextChanged(string value)
    {
        if (TryParseValidDate(value, out var date))
            DateFrom = date;
    }

    partial void OnDateToTextChanged(string value)
    {
        if (TryParseValidDate(value, out var date))
            DateTo = date;
    }

    private static bool TryParseValidDate(string text, out DateOnly date)
    {
        date = default;
        if (string.IsNullOrWhiteSpace(text)) return false;

        if (text.Length != 10) return false;
        if (text[2] != '.' || text[5] != '.') return false;

        if (!int.TryParse(text.AsSpan(0, 2), out var day)) return false;
        if (!int.TryParse(text.AsSpan(3, 2), out var month)) return false;
        if (!int.TryParse(text.AsSpan(6, 4), out var year)) return false;

        if (day < 1 || day > 31) return false;
        if (month < 1 || month > 12) return false;
        if (year < MinYear || year > MaxYear) return false;

        try
        {
            date = new DateOnly(year, month, day);
            return true;
        }
        catch
        {
            return false;
        }
    }

    [RelayCommand]
    private void SearchEmployee()
    {
        SearchError = string.Empty;
        SelectedEmployee = null;
        EmployeeFound = false;

        if (string.IsNullOrWhiteSpace(PassportSearch))
        {
            SearchError = "Введите номер паспорта";
            return;
        }

        var employee = DatabaseService.GetByPassport(PassportSearch.Trim());
        if (employee is null)
        {
            SearchError = "Сотрудник не найден";
            return;
        }

        SelectedEmployee = employee;
        EmployeeFound = true;
    }

    [RelayCommand]
    private void AddWorkAddress()
    {
        if (string.IsNullOrWhiteSpace(City) || string.IsNullOrWhiteSpace(Address))
        {
            Error = "Город и адрес обязательны";
            return;
        }

        WorkAddresses.Add(new WorkAddress
        {
            City = City.Trim(),
            Address = Address.Trim(),
            Object = WorkObject.Trim()
        });

        City = string.Empty;
        Address = string.Empty;
        WorkObject = string.Empty;
        Error = string.Empty;
    }

    [RelayCommand]
    private void RemoveWorkAddress(WorkAddress? address)
    {
        if (address is not null)
            WorkAddresses.Remove(address);
    }

    [RelayCommand]
    private void LoadReceiptPhoto()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
            Title = "Выберите фото росписки"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                var bytes = File.ReadAllBytes(dialog.FileName);
                ReceiptPhoto = bytes;

                // Обновляем превью в UI потоке
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ReceiptPhotoPreview = LoadPhotoFromBytes(bytes);
                });
            }
            catch (Exception ex)
            {
                Error = $"Ошибка загрузки фото: {ex.Message}";
            }
        }
    }

    [RelayCommand]
    private void DeleteReceiptPhoto()
    {
        ReceiptPhoto = null;
        ReceiptPhotoPreview = null;
    }

    [RelayCommand]
    private void SavePayment()
    {
        Error = Validate();
        if (!string.IsNullOrEmpty(Error)) return;

        var payment = new SalaryPayment
        {
            EmployeeId = SelectedEmployee!.Id,
            EmployeeName = SelectedEmployee.FullName,
            PassportNumber = SelectedEmployee.PassportNumber,
            DateFrom = DateFrom,
            DateTo = DateTo,
            WorkAddresses = WorkAddresses.ToList(),
            ReceiptPhoto = ReceiptPhoto,
            Comment = Comment
        };

        DatabaseService.AddPayment(payment);

        // Сброс формы
        PassportSearch = string.Empty;
        SelectedEmployee = null;
        EmployeeFound = false;
        WorkAddresses.Clear();
        ReceiptPhoto = null;
        ReceiptPhotoPreview = null;
        Comment = string.Empty;
        DateFrom = DateOnly.FromDateTime(DateTime.Today);
        DateTo = DateOnly.FromDateTime(DateTime.Today);
        DateFromText = DateFrom.ToString("dd.MM.yyyy");
        DateToText = DateTo.ToString("dd.MM.yyyy");
    }

    private string Validate()
    {
        var errors = new List<string>();

        if (SelectedEmployee is null)
            errors.Add("Сотрудник не выбран");

        if (!TryParseValidDate(DateFromText, out var from))
            errors.Add("Неверная дата начала. Формат: дд.мм.гггг (день 01-31, месяц 01-12, год 2004-5000)");
        if (!TryParseValidDate(DateToText, out var to))
            errors.Add("Неверная дата окончания. Формат: дд.мм.гггг (день 01-31, месяц 01-12, год 2004-5000)");

        if (from > to)
            errors.Add("Дата начала не может быть позже даты окончания");

        if (WorkAddresses.Count == 0)
            errors.Add("Добавьте хотя бы один адрес работы");

        if (Comment.Length > MaxCommentLength)
            errors.Add($"Комментарий не должен превышать {MaxCommentLength} символов");

        return errors.Count > 0 ? string.Join(Environment.NewLine, errors) : string.Empty;
    }

    private static BitmapSource LoadPhotoFromBytes(byte[] bytes)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.StreamSource = new MemoryStream(bytes);
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }
}