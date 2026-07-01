using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmployeeWorkTracker.Models;
using EmployeeWorkTracker.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EmployeeWorkTracker.ViewModels;

public partial class SalaryPaymentViewModel : ObservableObject
{
    [ObservableProperty] private string _passportSearch = string.Empty;
    [ObservableProperty] private string _searchError = string.Empty;
    [ObservableProperty] private bool _employeeFound;
    [ObservableProperty] private Employee? _selectedEmployee;
    [ObservableProperty] private string _dateFromText = string.Empty;
    [ObservableProperty] private string _dateToText = string.Empty;
    [ObservableProperty] private string _city = string.Empty;
    [ObservableProperty] private string _address = string.Empty;
    [ObservableProperty] private string _workObject = string.Empty;
    [ObservableProperty] private ObservableCollection<WorkAddress> _workAddresses = new();
    [ObservableProperty] private string _comment = string.Empty;
    [ObservableProperty] private string _error = string.Empty;
    [ObservableProperty] private byte[]? _receiptPhoto;
    [ObservableProperty] private ImageSource? _receiptPhotoPreview;
    [ObservableProperty] private ObservableCollection<byte[]> _receiptPhotos = new();
    [ObservableProperty] private ObservableCollection<ImageSource> _receiptPhotoPreviews = new();

    public int CommentCounter => Comment.Length;
    public bool IsCommentAtMax => Comment.Length >= 3000;

    partial void OnCommentChanged(string value)
    {
        OnPropertyChanged(nameof(CommentCounter));
        OnPropertyChanged(nameof(IsCommentAtMax));
    }

    [RelayCommand]
    private void SearchEmployee()
    {
        SearchError = string.Empty;

        if (string.IsNullOrWhiteSpace(PassportSearch))
        {
            SearchError = "Введите номер паспорта";
            EmployeeFound = false;
            return;
        }

        var passport = PassportSearch.Trim().ToUpperInvariant();
        var employee = DatabaseService.Employees.FirstOrDefault(e => e.PassportNumber.ToUpperInvariant() == passport);

        if (employee is null)
        {
            SearchError = "Сотрудник не найден";
            EmployeeFound = false;
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
            City = City,
            Address = Address,
            Object = WorkObject
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
        {
            WorkAddresses.Remove(address);
        }
    }

    [RelayCommand]
    private void LoadReceiptPhoto()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
            Title = "Выберите фото росписки",
            Multiselect = true
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                foreach (var fileName in dialog.FileNames)
                {
                    var bytes = File.ReadAllBytes(fileName);
                    ReceiptPhotos.Add(bytes);

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = new MemoryStream(bytes);
                    bitmap.EndInit();
                    bitmap.Freeze();
                    ReceiptPhotoPreviews.Add(bitmap);
                }

                if (ReceiptPhotoPreviews.Count > 0)
                {
                    ReceiptPhotoPreview = ReceiptPhotoPreviews[0];
                }
            }
            catch (Exception ex)
            {
                Error = $"Ошибка загрузки фото: {ex.Message}";
            }
        }
    }

    [RelayCommand]
    private void DeleteReceiptPhoto(ImageSource? photoSource)
    {
        if (photoSource is null) return;

        var index = ReceiptPhotoPreviews.IndexOf(photoSource);
        if (index >= 0 && index < ReceiptPhotos.Count)
        {
            ReceiptPhotos.RemoveAt(index);
            ReceiptPhotoPreviews.RemoveAt(index);

            if (ReceiptPhotoPreviews.Count > 0)
            {
                ReceiptPhotoPreview = ReceiptPhotoPreviews[0];
            }
            else
            {
                ReceiptPhotoPreview = null;
            }
        }
    }

    [RelayCommand]
    private void SavePayment()
    {
        Error = string.Empty;

        if (SelectedEmployee is null)
        {
            Error = "Сотрудник не выбран";
            return;
        }

        if (!DateOnly.TryParseExact(DateFromText, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var dateFrom))
        {
            Error = "Неверный формат даты начала (дд.мм.гггг)";
            return;
        }

        if (!DateOnly.TryParseExact(DateToText, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var dateTo))
        {
            Error = "Неверный формат даты окончания (дд.мм.гггг)";
            return;
        }

        if (dateFrom > dateTo)
        {
            Error = "Дата начала не может быть больше даты окончания";
            return;
        }

        if (WorkAddresses.Count == 0)
        {
            Error = "Добавьте хотя бы одно место работы";
            return;
        }

        if (ReceiptPhotos.Count == 0)
        {
            Error = "Добавьте фото росписки";
            return;
        }

        var payment = new SalaryPayment
        {
            EmployeeId = SelectedEmployee.Id,
            EmployeeName = SelectedEmployee.FullName,
            PassportNumber = SelectedEmployee.PassportNumber,
            DateFrom = dateFrom,
            DateTo = dateTo,
            WorkAddresses = WorkAddresses.ToList(),
            ReceiptPhotos = ReceiptPhotos.ToList(),
            Comment = Comment,
            CreatedAt = DateTime.Now
        };

        DatabaseService.AddPayment(payment);

        ClearForm();
    }

    private void ClearForm()
    {
        PassportSearch = string.Empty;
        SearchError = string.Empty;
        EmployeeFound = false;
        SelectedEmployee = null;
        DateFromText = string.Empty;
        DateToText = string.Empty;
        City = string.Empty;
        Address = string.Empty;
        WorkObject = string.Empty;
        WorkAddresses.Clear();
        Comment = string.Empty;
        ReceiptPhoto = null;
        ReceiptPhotoPreview = null;
        ReceiptPhotos.Clear();
        ReceiptPhotoPreviews.Clear();
        Error = string.Empty;
    }
}