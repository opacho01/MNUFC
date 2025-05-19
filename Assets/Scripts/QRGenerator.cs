using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

/// <summary>
/// Generates a QR code from a given URL and displays it on a RawImage.
/// </summary>
public class QRGenerator : MonoBehaviour
{
    /// <summary>
    /// The RawImage component where the generated QR code will be displayed.
    /// </summary>
    public RawImage QRFin;

    /// <summary>
    /// Sets the generated QR code texture to the RawImage.
    /// </summary>
    /// <param name="URL">The URL to encode into a QR code.</param>
    public void setURLtoQR(string URL)
    {
        Texture2D myQR = generateQR(URL);
        QRFin.texture = myQR;
    }

    /// <summary>
    /// Encodes a given text string into a QR code bitmap with specified dimensions.
    /// </summary>
    /// <param name="textForEncoding">The text or URL to encode into a QR code.</param>
    /// <param name="width">The width of the QR code.</param>
    /// <param name="height">The height of the QR code.</param>
    /// <returns>An array of Color32 representing the QR code pixels.</returns>
    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    /// <summary>
    /// Generates a QR code texture from a given text or URL.
    /// </summary>
    /// <param name="text">The text or URL to encode into a QR code.</param>
    /// <returns>A Texture2D containing the generated QR code.</returns>
    public Texture2D generateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }
}
