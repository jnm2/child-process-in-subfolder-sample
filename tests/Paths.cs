namespace SdkBuildTests;

internal static class Paths
{
    private static readonly Lazy<string> RepoRootFolder = new(() =>
    {
        for (var dir = AppContext.BaseDirectory; dir != null; dir = Path.GetDirectoryName(dir))
        {
            if (Directory.GetFiles(dir, "*.sln").Any())
                return dir;
        }

        throw new FileNotFoundException("No .sln file found in parent directories.");
    });

    public static string SrcFolder => Path.Join(RepoRootFolder.Value, "src");
}
