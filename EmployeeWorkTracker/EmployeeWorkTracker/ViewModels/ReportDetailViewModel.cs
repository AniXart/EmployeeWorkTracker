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
    public bool HasComment => !string.IsNullOrEmpty(_payment.Comment);

    public ObservableCollection<ImageSource> ReceiptPhotoSources { get; } = new();

    public bool HasReceiptPhotos => ReceiptPhotoSources.Count > 0;

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

    public ReportDetailViewModel(SalaryPayment payment)
    {
        _payment = payment;
        LoadReceiptPhotos();
        ReceiptPhotoSources.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasReceiptPhotos));
    }

    private void LoadReceiptPhotos()
    {
        ReceiptPhotoSources.Clear();

        if (_payment.ReceiptPhotos is not null)
        {
            foreach (var photoBytes in _payment.ReceiptPhotos)
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = new MemoryStream(photoBytes);
                    bitmap.EndInit();
                    bitmap.Freeze();
                    ReceiptPhotoSources.Add(bitmap);
                }
                catch { }
            }
        }
    }

    [RelayCommand]
    private void AddReceiptPhoto()
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
                _payment.ReceiptPhotos ??= new List<byte[]>();

                foreach (var fileName in dialog.FileNames)
                {
                    var bytes = File.ReadAllBytes(fileName);
                    _payment.ReceiptPhotos.Add(bytes);

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = new MemoryStream(bytes);
                    bitmap.EndInit();
                    bitmap.Freeze();
                    ReceiptPhotoSources.Add(bitmap);
                }

                UpdateDatabase();
                OnPropertyChanged(nameof(HasReceiptPhotos));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public void DeleteReceiptPhoto(int index)
    {
        if (_payment.ReceiptPhotos is not null && index >= 0 && index < _payment.ReceiptPhotos.Count)
        {
            _payment.ReceiptPhotos.RemoveAt(index);
            ReceiptPhotoSources.RemoveAt(index);
            UpdateDatabase();
            OnPropertyChanged(nameof(HasReceiptPhotos));
        }
    }

    private void UpdateDatabase()
    {
        DatabaseService.RemovePayment(_payment.Id);
        DatabaseService.AddPayment(_payment);
    }
}