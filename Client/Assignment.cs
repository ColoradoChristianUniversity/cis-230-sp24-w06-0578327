﻿using System.Globalization;

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
        ValidateParameters();

        // Implementation goes here.
        throw new NotImplementedException();

        void ValidateParameters()
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

        // Implementation goes here.
        throw new NotImplementedException();

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
    /// This deletes all files in the root directory.
    /// This deletes all directories/subdirectories in the root directory.
    /// </remarks>
    public void DeleteFiles()
    {
        if (!Directory.Exists(_rootDirectory))
        {
            return;
        }

        // Implementation goes here.
        throw new NotImplementedException();
    }
}