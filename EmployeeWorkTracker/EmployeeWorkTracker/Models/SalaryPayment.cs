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
    public List<byte[]>? ReceiptPhotos { get; set; } = new();
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    public string PeriodDisplay => $"{DateFrom:dd.MM.yyyy} — {DateTo:dd.MM.yyyy}";

    [JsonIgnore]
    public byte[]? EmployeePhoto => DatabaseService.GetById(EmployeeId)?.Photo;

    [JsonIgnore]
    public ImageSource? EmployeePhotoSource
    {
        get
        {
            var photo = EmployeePhoto;
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

    [JsonIgnore]
    public ImageSource? ReceiptPhotoSource
    {
        get
        {
            byte[]? photoBytes = null;

            if (ReceiptPhotos is not null && ReceiptPhotos.Count > 0)
            {
                photoBytes = ReceiptPhotos[0];
            }
            else if (ReceiptPhoto is not null && ReceiptPhoto.Length > 0)
            {
                photoBytes = ReceiptPhoto;
            }

            if (photoBytes is null || photoBytes.Length == 0) return null;

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new MemoryStream(photoBytes);
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch { return null; }
        }
    }

    [JsonIgnore]
    public List<ImageSource> ReceiptPhotoSources
    {
        get
        {
            var sources = new List<ImageSource>();
            var photos = ReceiptPhotos;

            if (photos is null || photos.Count == 0)
            {
                if (ReceiptPhoto is not null && ReceiptPhoto.Length > 0)
                {
                    photos = new List<byte[]> { ReceiptPhoto };
                }
            }

            if (photos is not null)
            {
                foreach (var photoBytes in photos)
                {
                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = new MemoryStream(photoBytes);
                        bitmap.EndInit();
                        bitmap.Freeze();
                        sources.Add(bitmap);
                    }
                    catch { }
                }
            }
            return sources;
        }
    }
}