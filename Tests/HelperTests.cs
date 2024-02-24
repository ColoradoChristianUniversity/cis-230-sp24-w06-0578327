using Xunit;
using System.IO;
using SkiaSharp;

namespace Tests
{
    public class HelperTests
    {
        [Fact]
        public void WriteImageFile_CreatesDirectory()
        {
            // Arrange
            string directoryPath = RandomDirectoryPath;
            string fileName = "testImage.png";
            int imageHeight = 100;
            int imageWidth = 200;

            // Act
            Helper.WriteImageFile(directoryPath, fileName, imageHeight, imageWidth);

            // Assert
            Assert.True(Directory.Exists(directoryPath));

            // Cleanup
            SafeDirectoryDelete(directoryPath);
        }

        [Fact]
        public void WriteImageFile_CreatesImage()
        {
            // Arrange
            string directoryPath = RandomDirectoryPath;
            string fileName = "testImage.png";
            int imageHeight = 100;
            int imageWidth = 200;

            // Act
            Helper.WriteImageFile(directoryPath, fileName, imageHeight, imageWidth);

            // Assert
            string filePath = Path.Combine(directoryPath, fileName);
            Assert.True(File.Exists(filePath));

            // Cleanup
            SafeDirectoryDelete(directoryPath);
        }

        [Fact]
        public void WriteImageFile_CreatesImageWithCorrectDimensions()
        {
            // Arrange
            string directoryPath = RandomDirectoryPath;
            string fileName = "testImage.png";
            int imageHeight = 100;
            int imageWidth = 200;

            // Act
            Helper.WriteImageFile(directoryPath, fileName, imageHeight, imageWidth);

            // Assert
            string filePath = Path.Combine(directoryPath, fileName);
            using var image = SKBitmap.Decode(filePath);
            Assert.Equal(imageHeight, image.Height);
            Assert.Equal(imageWidth, image.Width);

            // Cleanup
            SafeDirectoryDelete(directoryPath);
        }

        [Fact]
        public void WriteImageFile_ThrowsDirectoryNotFoundException_WhenDirectoryDoesNotExistAndCreateDirectoryIsFalse()
        {
            // Arrange
            string directoryPath = RandomDirectoryPath; // Non-existing directory
            string fileName = "testImage.png";

            // Act & Assert
            var exception = Assert.Throws<DirectoryNotFoundException>(() => Helper.WriteImageFile(directoryPath, fileName, createDirectory: false));
        }

        [Fact]
        public void WriteImageFile_ThrowsArgumentException_ForInvalidFileName()
        {
            // Arrange
            var invalidChars = Path.GetInvalidFileNameChars();
            string directoryPath = RandomDirectoryPath;
            string fileName = "test" + invalidChars[0] + "Image.png"; // Constructing an invalid filename

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Helper.WriteImageFile(directoryPath, fileName));
        }

        [Theory]
        [InlineData(0, 100)] // Invalid height, valid width
        [InlineData(100, 0)] // Valid height, invalid width
        [InlineData(-1, 100)] // Negative height, valid width
        [InlineData(100, -1)] // Valid height, negative width
        [InlineData(0, 0)] // Both height and width are invalid
        public void WriteImageFile_ThrowsArgumentOutOfRangeException_ForInvalidDimensions(int imageHeight, int imageWidth)
        {
            // Arrange
            string directoryPath = RandomDirectoryPath;
            string fileName = "testImage.png";

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Helper.WriteImageFile(directoryPath, fileName, imageHeight: imageHeight, imageWidth: imageWidth));
        }

        [Theory]
        [InlineData("jpg")]
        [InlineData(".jpg")]
        [InlineData("txt")]
        [InlineData(".txt")]
        public void GetRandomFilename_ReturnsWithCorrectExtension(string expectedExtension)
        {
            // Act
            string fileName = Helper.GetRandomFilename(expectedExtension);

            // Assert
            Assert.EndsWith($".{expectedExtension.Trim('.')}", fileName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetRandomFilename_ThrowsArgumentException_WhenExtensionIsNullOrEmpty(string extension)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Helper.GetRandomFilename(extension));
            Assert.Equal("extension", exception.ParamName);
        }

        [Theory]
        [InlineData(".")]
        [InlineData(" ")]
        [InlineData("  .  ")]
        public void GetRandomFilename_ThrowsArgumentException_WhenExtensionIsWhitespace(string extension)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Helper.GetRandomFilename(extension));
            Assert.Equal("extension", exception.ParamName);
        }

        [Fact]
        public void GetRandomFilename_ThrowsArgumentException_WhenExtensionContainsInvalidCharacters()
        {
            // Arrange
            var invalidChars = Path.GetInvalidFileNameChars();

            foreach (var invalidChar in invalidChars)
            {
                string extension = $"ext{invalidChar}nsion";

                // Act & Assert
                var exception = Assert.Throws<ArgumentException>(() => Helper.GetRandomFilename(extension));
                Assert.Equal("extension", exception.ParamName);
            }
        }

        private static string RandomDirectoryPath => Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        private static void SafeDirectoryDelete(string directoryPath, int maxAttempts = 10, int delayBetweenAttempts = 100)
        {
            if (!Directory.Exists(directoryPath))
            {
                return; // Nothing to do
            }

            if (directoryPath == Path.GetTempPath())
            {
                return; // Don't delete the system temp directory
            }

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    Directory.Delete(directoryPath, true);
                    return;
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException && attempt < maxAttempts)
                {
                    Thread.Sleep(delayBetweenAttempts);
                }
            }

            throw new IOException($"Failed to delete directory '{directoryPath}' after {maxAttempts} attempts.");
        }
    }
}