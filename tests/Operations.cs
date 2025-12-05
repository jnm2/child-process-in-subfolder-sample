using System.Diagnostics;
using System.Text;

namespace SdkBuildTests;

internal static class Operations
{
    /// <param name="sourceDirectory">The directory whose contents will be recursively copied.</param>
    /// <param name="destinationDirectory">Will be created if it does not exist, regardless of whether files are copied.</param>
    public static async Task CopyGitTrackedFilesAsync(string sourceDirectory, string destinationDirectory)
    {
        var result = await RunProcessAsync("git", ["ls-files"], workingDirectory: sourceDirectory);
        result.ThrowIfNonZeroOrStdErr();

        foreach (var filePath in result.StdOut.EnumerateLines())
        {
            var sourcePath = Path.Join(sourceDirectory, filePath);
            var destinationPath = Path.Join(destinationDirectory, filePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
            File.Copy(sourcePath, destinationPath);
        }
    }

    public static async Task<ProcessRunResult> RunProcessAsync(string fileName, IEnumerable<string> arguments, string workingDirectory)
    {
        using var process = Process.Start(new ProcessStartInfo(fileName, arguments)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        })!;

        process.StandardInput.Close();

        var outputBuilder = (StringBuilder?)null;

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is null)
                return;

            if (outputBuilder is null)
                outputBuilder = new StringBuilder();
            else
                outputBuilder.AppendLine();

            outputBuilder.Append(e.Data);
        };

        var errorBuilder = (StringBuilder?)null;

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is null)
                return;

            if (errorBuilder is null)
                errorBuilder = new StringBuilder();
            else
                errorBuilder.AppendLine();

            errorBuilder.Append(e.Data);
        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        return new ProcessRunResult
        {
            ProcessName = fileName,
            ExitCode = process.ExitCode,
            StdOut = outputBuilder?.ToString() ?? "",
            StdErr = errorBuilder?.ToString() ?? "",
        };
    }
}