using System.Globalization;
using QRCoder;

namespace Nuttawat_7324_L002.Services;

public static class MockPaymentQr
{
    public static string Reference(string revision) => $"DEMO-{revision[..Math.Min(12, revision.Length)].ToUpperInvariant()}";

    public static string Payload(string reference, decimal total) =>
        $"CustomShop DEMO ONLY / {reference} / THB {total.ToString("F2", CultureInfo.InvariantCulture)}";

    public static string ImageDataUrl(string reference, decimal total)
    {
        using var data = QRCodeGenerator.GenerateQrCode(Payload(reference, total), QRCodeGenerator.ECCLevel.M);
        using var image = new PngByteQRCode(data);
        return "data:image/png;base64," + Convert.ToBase64String(image.GetGraphic(8));
    }
}
