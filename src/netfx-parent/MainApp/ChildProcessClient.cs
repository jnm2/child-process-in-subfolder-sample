using System;
using System.Diagnostics;
using System.IO;

namespace MainApp;

public sealed class ChildProcessClient : IDisposable
{
    public ChildProcessClient()
    {
        var childProcessSubfolder = Path.Combine(AppContext.BaseDirectory, "ChildProcessSubfolder");

        using var process = Process.Start(new ProcessStartInfo
        {
            UseShellExecute = false,
            WorkingDirectory = childProcessSubfolder,
            FileName = Path.Combine(childProcessSubfolder, "ChildProcess.exe"),
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
        })!;

        StandardInput = process.StandardInput;
        StandardOutput = process.StandardOutput;
    }

    public StreamWriter StandardInput { get; }
    public StreamReader StandardOutput { get; }

    public void Dispose()
    {
        StandardInput.Dispose();
        StandardOutput.Dispose();
    }
}
