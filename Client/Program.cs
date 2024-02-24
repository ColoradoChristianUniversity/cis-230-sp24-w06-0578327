using System.Diagnostics;
using System.Runtime.InteropServices;

var directoryPath = Path.Combine(Path.GetTempPath(), "ccu-files");
var assignment = new Assignment(directoryPath);
var menu = new Menu(new(0, 1), "Create Files", "Move Files", "Delete Files", "Open Directory", "Count files");

Console.Clear();
WriteInfo("Welcome to the File Management System!");

while (true)
{
    var index = menu.Show();

    if (index == 0)
    {
        return; // this means exit the program
    }
    else if (index == 1)
    {
        CreateFiles(assignment);
    }
    else if (index == 2)
    {
        MoveFiles(assignment);
    }
    else if (index == 3)
    {
        DeleteFiles(assignment);
    }
    else if (index == 4)
    {
        OpenDirectory(directoryPath);
    }
    else if (index == 5)
    {
        // there are {x} files in the root and {y} files in the subdirectories
        CountFiles(out var root, out var subs);
        WriteInfo($"There are {root} files in the root, {subs} in subdirectories.");
    }

    void CountFiles(out int root, out int subs)
    {
        root = Directory.GetFiles(directoryPath).Length;
        subs = Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories).Sum(d => Directory.GetFiles(d).Length);
        subs = Math.Max(0, subs - root);
    }
}

static void CreateFiles(Assignment assignment)
{
    WriteInfo("Creating files...");
    try
    {
        assignment.CreateFiles();
        WriteInfo("Files have been created.");
    }
    catch (Exception e)
    {
        WriteError($"{e.Message}");
    }
}

static void MoveFiles(Assignment assignment)
{
    WriteInfo("Moving files...");
    try
    {
        assignment.MoveFiles();
        WriteInfo("Files have been moved.");
    }
    catch (Exception e)
    {
        WriteError($"{e.Message}");
    }
}

static void DeleteFiles(Assignment assignment)
{
    WriteInfo("Deleting files...");
    try
    {
        assignment.DeleteFiles();
        WriteInfo("Files have been deleted.");
    }
    catch (Exception e)
    {
        WriteError($"{e.Message}");
    }
}

static void OpenDirectory(string directoryPath)
{
    // ensure the directory exists
    Directory.CreateDirectory(directoryPath);

    WriteInfo($"Opening {directoryPath}.");
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        Process.Start("explorer.exe", directoryPath);
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
        Process.Start("open", "-R " + directoryPath);
    }
    WriteInfo($"{directoryPath} opened.");
}

static void WriteInfo(string message) => WriteMessage(message, foreground: ConsoleColor.Green);

static void WriteError(string message) => WriteMessage(message, foreground: ConsoleColor.Red);

static void WriteMessage(string message, ConsoleColor? background = null, ConsoleColor? foreground = null)
{
    Helper.Write(message, 0, 0, background, foreground, clearFirst: true);

    if (string.IsNullOrWhiteSpace(message))
    {
        return;
    }

    _ = new Timer(_ => WriteInfo(string.Empty), null, 2000, Timeout.Infinite);
}