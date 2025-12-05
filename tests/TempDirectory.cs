using System.Diagnostics;

namespace SdkBuildTests;

[DebuggerDisplay("{ToString(),nq}")]
internal sealed class TempDirectory : IDisposable
{
    private string? path;
    public string Path => path ?? throw new ObjectDisposedException(nameof(TempDirectory));

    public TempDirectory()
    {
        path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
        Directory.CreateDirectory(path);
    }

    public string GetFullPath(string relativePath)
    {
        return System.IO.Path.Join(Path, relativePath);
    }

    public void Dispose()
    {
        var path = Interlocked.Exchange(ref this.path, null);
        if (path is null) return;

        try
        {
            Directory.Delete(path, true);
        }
        catch (IOException) { }
    }

    public override string ToString() => path ?? "(disposed)";
}
