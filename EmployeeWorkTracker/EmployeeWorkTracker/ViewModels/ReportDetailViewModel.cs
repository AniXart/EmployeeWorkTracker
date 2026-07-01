using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmployeeWorkTracker.Models;
using EmployeeWorkTracker.Services;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EmployeeWorkTracker.ViewModels;

public sealed partial class ReportDetailViewModel : ObservableObject
{
    private readonly SalaryPayment _payment;

    public string EmployeeName => _payment.EmployeeName;
    public string PassportNumber => _payment.PassportNumber;
    public string Position => DatabaseService.GetById(_payment.EmployeeId)?.Position ?? string.Empty;
    public string PeriodDisplay => _payment.PeriodDisplay;
    public List<WorkAddress> WorkAddresses => _payment.WorkAddresses;
    public string Comment => _payment.Comment;
    public string CreatedAtText => $"Создано: {_payment.CreatedAt:dd.MM.yyyy HH:mm}";
    public bool HasWorkAddresses => _payment.WorkAddresses.Count > 0;
    public bool HasReceiptPhoto => _payment.ReceiptPhoto is not null && _payment.ReceiptPhoto.Length > 0;
    public bool HasComment => !string.IsNullOrEmpty(_payment.Comment);

    public ImageSource? EmployeePhotoSource
    {
        get
        {
            var photo = DatabaseService.GetById(_payment.EmployeeId)?.Photo;
            if (photo is null || photo.Length == 0) return null;
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new MemoryStream(photo);
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch { return null; }
        }
    }

    public ImageSource? ReceiptPhotoSource
    {
        get
        {
            if (_payment.ReceiptPhoto is null || _payment.ReceiptPhoto.Length == 0) return null;
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new MemoryStream(_payment.ReceiptPhoto);
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch { return null; }
        }
    }

    public ReportDetailViewModel(SalaryPayment payment)
    {
        _payment = payment;
    }

    [RelayCommand]
    private void DeleteReceiptPhoto()
    {
        _payment.ReceiptPhoto = null;
        UpdateDatabase();
        OnPropertyChanged(nameof(HasReceiptPhoto));
        OnPropertyChanged(nameof(ReceiptPhotoSource));
    }

    [RelayCommand]
    private void ChangeReceiptPhoto()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
            Title = "Выберите новое фото росписки"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                var bytes = File.ReadAllBytes(dialog.FileName);
                _payment.ReceiptPhoto = bytes;
                UpdateDatabase();
                OnPropertyChanged(nameof(HasReceiptPhoto));
                OnPropertyChanged(nameof(ReceiptPhotoSource));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }

    private void UpdateDatabase()
    {
        DatabaseService.RemovePayment(_payment.Id);
        DatabaseService.AddPayment(_payment);
    }
}