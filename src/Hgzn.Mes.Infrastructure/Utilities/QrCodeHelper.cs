using Microsoft.Extensions.Logging;
using QRCoder;
using SkiaSharp;

namespace Hgzn.Mes.Infrastructure.Utilities;

public static class QrCodeHelper
{
    private static async Task CreateQrCode(string content, string filePath)
    {
        using QRCodeGenerator qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new BitmapByteQRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(20);
        using var ms = new MemoryStream(qrCodeImage);
        using var bitmap = SKBitmap.Decode(ms);
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        bitmap.Encode(fileStream, SKEncodedImageFormat.Png, 100);
    }

    public static async Task<string> GetOrCreateQrCode(string name, string content, string pathName)
    {
        var path = Path.Combine(Environment.CurrentDirectory, pathName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        var filePath = Path.Combine(path, $"{name}.png");
        if (!File.Exists(filePath))
        {
            await CreateQrCode(content, filePath);
        }
        var imageBytes = await File.ReadAllBytesAsync(filePath);
        var base64String = Convert.ToBase64String(imageBytes);
        return "data:image/jpeg;base64," + base64String;
    }
}