using System.Security.Cryptography.X509Certificates;
using System.Text;

using SkiaSharp;

public static class Helper
{
    public static void WriteImageFile(string directoryPath,
                                      string fileName,
                                      int imageHeight = 100,
                                      int imageWidth = 100,
                                      bool createDirectory = true,
                                      bool overwriteFile = false)
    {
        ValidateParameters(out var path);

        // Create the image
        using var bitmap = new SKBitmap(imageWidth, imageHeight);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.White);

        // Draw the text
        var textFormatting = TextFormatting();
        var (textX, textY) = TextCenteredCoordinates(imageHeight, imageWidth, textFormatting);
        var text = fileName.Split(".").FirstOrDefault() ?? fileName;
        canvas.DrawText(text, textX, textY, textFormatting);

        // Save the image
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.Create(path);
        data.SaveTo(stream);

        static (float X, float Y) TextCenteredCoordinates(int bitmapHeight, int bitmapWidth, SKPaint textPaint)
        {
            var metrics = textPaint.FontMetrics;
            var textX = (float)(bitmapWidth / 2);
            var textY = (float)(bitmapHeight / 2 - (metrics.Ascent + metrics.Descent) / 2 - metrics.Leading);
            return (textX, textY);
        }

        static SKPaint TextFormatting()
        {
            return new SKPaint
            {
                Color = SKColors.Red,
                TextSize = 12,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center,
                Typeface = SKTypeface.FromFamilyName("Arial")
            };
        }

        void ValidateParameters(out string path)
        {
            if (!Directory.Exists(directoryPath) && createDirectory)
            {
                Directory.CreateDirectory(directoryPath);
            }
            else if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
            }

            var invalidFileNameChars = Path.GetInvalidFileNameChars();
            if (fileName.Any(c => invalidFileNameChars.Contains(c)))
            {
                throw new ArgumentException($"Filename '{fileName}' contains invalid characters.", nameof(fileName));
            }

            var baseFileName = Path.GetFileNameWithoutExtension(fileName);
            var fileExtension = Path.GetExtension(fileName);
            path = Path.Combine(directoryPath, $"{baseFileName}{fileExtension}");

            var counter = 2;
            while (File.Exists(path) && !overwriteFile)
            {
                path = Path.Combine(directoryPath, $"{baseFileName} ({counter++}){fileExtension}");
            }

            if (imageHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(imageHeight), "Image height must be greater than 0.");
            }

            if (imageWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(imageWidth), "Image width must be greater than 0.");
            }
        }
    }

    public static string GetRandomFilename(string extension = "txt")
    {
        extension = ValidateParameters();

        var number = Random.Shared.Next(0, 1000);
        var date = DateTime.Now.AddDays(-number);
        return $"{date:yyyy-MM-dd}.{extension}";

        string ValidateParameters()
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException("Extension cannot be null or empty.", nameof(extension));
            }

            if (extension.Replace(" ", string.Empty).Replace(".", string.Empty) == string.Empty)
            {
                throw new ArgumentException("Extension cannot consist only of whitespace or dots.", nameof(extension));
            }

            var invalidChars = Path.GetInvalidFileNameChars();
            if (extension.Any(c => invalidChars.Contains(c)))
            {
                throw new ArgumentException($"Extension contains invalid characters: {new string(invalidChars)}", nameof(extension));
            }

            return extension.Trim(' ').Trim('.');
        }
    }

    public static void Write(string text, int x, int y, ConsoleColor? background = null, ConsoleColor? foreground = null, bool clearFirst = false)
    {
        lock (Console.Out)
        {
            Console.SetCursorPosition(x, y);
            if (clearFirst)
            {
                Console.Write(new string(' ', Console.WindowWidth - x));
            }

            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = background ?? Console.BackgroundColor;
            Console.ForegroundColor = foreground ?? Console.ForegroundColor;
            Console.Write(text);
            Console.ResetColor();
        }
    }
}