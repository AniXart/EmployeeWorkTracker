using EmployeeWorkTracker.Services;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EmployeeWorkTracker.Models;

public sealed class WorkAddress
{
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Object { get; set; } = string.Empty;
}

public sealed class SalaryPayment
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public List<WorkAddress> WorkAddresses { get; set; } = new();
    public byte[]? ReceiptPhoto { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    public string PeriodDisplay => $"{DateFrom:dd.MM.yyyy} — {DateTo:dd.MM.yyyy}";

    [JsonIgnore]
    public byte[]? EmployeePhoto => DatabaseService.GetById(EmployeeId)?.Photo;

    [JsonIgnore]
    public ImageSource? ReceiptPhotoSource
    {
        get
        {
            if (ReceiptPhoto is null || ReceiptPhoto.Length == 0)
                return null;

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(ReceiptPhoto);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
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

    [JsonIgnore]
    public ImageSource? EmployeePhotoSource
    {
        get
        {
            var photo = EmployeePhoto;
            if (photo is null || photo.Length == 0)
                return null;

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(photo);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
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
}