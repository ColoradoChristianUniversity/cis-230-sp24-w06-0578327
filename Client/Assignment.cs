using System.Globalization;

public class Assignment : IAssignment
{
    private readonly string _rootDirectory;

    public Assignment(string rootDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootDirectory);

        _rootDirectory = rootDirectory;
    }

    /// <summary>
    /// Creates files in the root directory, each named with a unique timestamp.
    /// </summary>
    /// <param name="count">The number of files to create.</param>
    /// <remarks>
    /// This method uses Helpers.GetRandomFilename to generate the file names.
    /// This method usees Helpers.WriteImageFile to create the files.
    /// </remarks>
    public void CreateFiles(int count = 10)
    {
        ValidateParameters(count);

        // Implementation goes here.
        for (int i = 0; i < count; i++)
        {
            var filename = Helper.GetRandomFilename("png");
            Helper.WriteImageFile(_rootDirectory, filename);
        }

        static void ValidateParameters(int count)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0.");
            }
        }
    }

    /// <summary>
    /// Reads the timestamps from file names and organizes the files into subdirectories by year and month.
    /// </summary>
    /// <remarks>
    /// This method builds a subdirectories structure like: root/year/month
    /// </remarks>
    public void MoveFiles()
    {
        ValidatePreconditions();

        foreach (var sourceFile in Directory.GetFiles(_rootDirectory).Where(x => x.Length > 10))
        {
            var filename = Path.GetFileName(sourceFile);
            var destinationFile = Path.Combine(_rootDirectory, filename[..4], filename[5..7], filename);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
            File.Move(sourceFile, destinationFile, true);
        }

        void ValidatePreconditions()
        {
            if (!Directory.Exists(_rootDirectory))
            {
                throw new DirectoryNotFoundException($"The directory '{_rootDirectory}' does not exist.");
            }

            if (Directory.GetFiles(_rootDirectory).Length == 0)
            {
                throw new Exception($"The directory '{_rootDirectory}' does not contain any files.");
            }
        }
    }

    /// <summary>
    /// Deletes all files and directories created by this assignment, including the root directory.
    /// </summary>
    /// <remarks>
    /// This deletes all files and directories in the root directory, so be careful!
    /// </remarks>
    public void DeleteFiles()
    {
        if (!Directory.Exists(_rootDirectory))
        {
            return;
        }

        // Implementation goes here.

        foreach (var file in Directory.GetFiles(_rootDirectory))
        {
            File.Delete(file);
        }
        foreach (var directory in Directory.GetDirectories(_rootDirectory))
        {
            Directory.Delete(directory, true);
        }
    }
}
