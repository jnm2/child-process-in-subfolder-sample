namespace SdkBuildTests;

internal sealed record ProcessRunResult
{
    public required string ProcessName { get; init; }
    public required int ExitCode { get; init; }
    public required string StdOut { get; init; }
    public required string StdErr { get; init; }

    public void ThrowIfNonZeroOrStdErr()
    {
        if (ExitCode != 0 || StdErr != "")
            throw new InvalidOperationException($"'{ProcessName}' exited with code {ExitCode}.\nStdOut:\n{StdOut}\n\nStdErr:\n{StdErr}");
    }
}
