using Microsoft.AspNetCore.Components.Forms;

namespace EventHotelBroker.Models;

public class UploadedFileData
{
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public string PreviewUrl { get; set; } = string.Empty;

    public static async Task<UploadedFileData?> FromBrowserFileAsync(IBrowserFile file, long maxFileSize = 5 * 1024 * 1024)
    {
        if (file == null || file.Size > maxFileSize) return null;
        using var ms = new MemoryStream();
        await file.OpenReadStream(maxFileSize).CopyToAsync(ms);
        var bytes = ms.ToArray();
        var base64 = Convert.ToBase64String(bytes);
        return new UploadedFileData
        {
            Name = file.Name,
            ContentType = file.ContentType,
            Size = file.Size,
            Data = bytes,
            PreviewUrl = $"data:{file.ContentType};base64,{base64}"
        };
    }
}
