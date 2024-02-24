using System.Reflection.Metadata.Ecma335;

namespace Tests;

public class AssignmentTests : IDisposable
{
    private readonly string _testDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    private readonly Assignment _assignment;

    public AssignmentTests()
    {
        Directory.CreateDirectory(_testDirectory);
        _assignment = new Assignment(_testDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }

    [Theory]
    [InlineData(5)]
    [InlineData(15)]
    public void CreateFiles_WithSpecificCount_CreatesSpecifiedNumberOfFiles(int count)
    {
        // act
        _assignment.CreateFiles(count);

        // assert
        var files = Directory.GetFiles(_testDirectory);
        Assert.Equal(count, files.Length);
    }

    [Fact]
    public void CreateFiles_WithZeroCount_ThrowsArgumentOutOfRangeException()
    {
        // act & assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _assignment.CreateFiles(0));
        Assert.Equal("count", exception.ParamName);
    }

    [Fact]
    public void MoveFiles_AfterCreatingFiles_DeletesOldFiles()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            var file = Helper.GetRandomFilename();
            Helper.WriteImageFile(_testDirectory, file);
        }

        // Act
        _assignment.MoveFiles();

        // Assert
        var files = Directory.GetFiles(_testDirectory, "*", SearchOption.TopDirectoryOnly);
        Assert.Empty(files);
    }

    [Fact]
    public void MoveFiles_AfterCreatingFiles_MovesFilesToCorrectSubdirectories()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            var file = Helper.GetRandomFilename();
            Helper.WriteImageFile(_testDirectory, file);
        }

        // Act
        _assignment.MoveFiles();

        // Assert
        foreach (var path in Directory.GetFiles(_testDirectory, "*", SearchOption.AllDirectories))
        {
            var filename = Path.GetFileName(path);

            var month = filename[5..7];
            var parent = Path.GetDirectoryName(path)!;
            Assert.Equal(month, parent[^2..]);

            var year = filename[..4];
            var grandparent = Path.GetDirectoryName(parent)!;
            Assert.Equal(year, grandparent[^4..]);
        }
    }

    [Fact]
    public void DeleteFiles_WhenCalled_DoesNotDeleteRoot()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            var file = Helper.GetRandomFilename();
            Helper.WriteImageFile(_testDirectory, file);
        }
        _assignment.MoveFiles();

        // Act
        _assignment.DeleteFiles();

        // Assert
        Assert.True(Directory.Exists(_testDirectory));
    }

    [Fact]
    public void DeleteFiles_WhenCalled_DeletesAllFiles()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            var file = Helper.GetRandomFilename();
            Helper.WriteImageFile(_testDirectory, file);
        }
        _assignment.MoveFiles();

        // Act
        _assignment.DeleteFiles();

        // Assert
        Assert.Empty(Directory.GetFiles(_testDirectory, "*", SearchOption.AllDirectories));
    }

    [Fact]
    public void DeleteFiles_WhenCalled_DeletesAllDirectories()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            var file = Helper.GetRandomFilename();
            Helper.WriteImageFile(_testDirectory, file);
        }
        _assignment.MoveFiles();

        // Act
        _assignment.DeleteFiles();

        // Assert
        Assert.Empty(Directory.GetDirectories(_testDirectory, "*", SearchOption.AllDirectories));
    }

    [Fact]
    public void DeleteFiles_WhenCalledOnNonExistentDirectory_DoesNotThrow()
    {
        // Arrange
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }

        // Act & Assert
        var exception = Record.Exception(_assignment.DeleteFiles);
        Assert.Null(exception);
    }
}