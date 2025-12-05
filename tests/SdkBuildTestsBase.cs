namespace SdkBuildTests;

[InheritsTests]
public class NetcoreParentSdkBuildTests() : SdkBuildTestsBase(srcSubfolder: "netcore-parent", parentTfm: "net9.0-windows");

[InheritsTests]
public class NetfxParentSdkBuildTests() : SdkBuildTestsBase(srcSubfolder: "netfx-parent", parentTfm: "net4.8");

public abstract class SdkBuildTestsBase(string srcSubfolder, string parentTfm)
{
    [Test, MatrixDataSource]
    public async Task Child_process_is_created_in_parent_output_subfolder(bool transitiveReference)
    {
        var projectName = transitiveReference ? "MainApp.Tests" : "MainApp";

        using var workingDirectory = await SetUpWorkingDirectoryAsync(srcSubfolder);

        (await Operations.RunProcessAsync("dotnet", ["build", projectName], workingDirectory.Path)).ThrowIfNonZeroOrStdErr();

        await Assert.That(File.Exists(workingDirectory.GetFullPath($@"{projectName}\bin\Debug\{parentTfm}\ChildProcessSubfolder\ChildProcess.exe"))).IsTrue();
        await Assert.That(File.Exists(workingDirectory.GetFullPath($@"{projectName}\bin\Debug\{parentTfm}\ChildProcessSubfolder\ChildProcess.pdb"))).IsTrue();
    }

    [Test, MatrixDataSource]
    public async Task Child_process_is_updated_in_parent_output_subfolder_when_building_parent_and_only_the_child_is_dirty(bool transitiveReference)
    {
        var projectName = transitiveReference ? "MainApp.Tests" : "MainApp";

        using var workingDirectory = await SetUpWorkingDirectoryAsync(srcSubfolder);

        // Initial build
        (await Operations.RunProcessAsync("dotnet", ["build", projectName], workingDirectory.Path)).ThrowIfNonZeroOrStdErr();

        var originalChildExe = await File.ReadAllBytesAsync(workingDirectory.GetFullPath(@"ChildProcess\bin\Debug\ChildProcess.exe"));
        var originalChildPdb = await File.ReadAllBytesAsync(workingDirectory.GetFullPath(@"ChildProcess\bin\Debug\ChildProcess.pdb"));

        // Cause the child project to need recompilation
        await File.AppendAllTextAsync(
            workingDirectory.GetFullPath(@"ChildProcess\Program.cs"),
            """

            // edit
            """);

        // Build again, testing incremental build
        (await Operations.RunProcessAsync("dotnet", ["build", projectName], workingDirectory.Path)).ThrowIfNonZeroOrStdErr();

        var rebuiltChildExe = await File.ReadAllBytesAsync(workingDirectory.GetFullPath(@"ChildProcess\bin\Debug\ChildProcess.exe"));
        var rebuiltChildPdb = await File.ReadAllBytesAsync(workingDirectory.GetFullPath(@"ChildProcess\bin\Debug\ChildProcess.pdb"));

        if (rebuiltChildExe.SequenceEqual(originalChildExe)) Assert.Fail("Test setup problem: exe was not rebuilt.");
        if (rebuiltChildPdb.SequenceEqual(originalChildPdb)) Assert.Fail("Test setup problem: pdb was not rebuilt.");

        var lastCopiedChildExe = await File.ReadAllBytesAsync(workingDirectory.GetFullPath($@"{projectName}\bin\Debug\{parentTfm}\ChildProcessSubfolder\ChildProcess.exe"));
        var lastCopiedChildPdb = await File.ReadAllBytesAsync(workingDirectory.GetFullPath($@"{projectName}\bin\Debug\{parentTfm}\ChildProcessSubfolder\ChildProcess.pdb"));

        if (!lastCopiedChildExe.SequenceEqual(rebuiltChildExe)) Assert.Fail("The rebuilt child .exe file was not copied to the parent subfolder.");
        if (!lastCopiedChildPdb.SequenceEqual(rebuiltChildPdb)) Assert.Fail("The rebuilt child .pdb file was not copied to the parent subfolder.");
    }

    private static async Task<TempDirectory> SetUpWorkingDirectoryAsync(string srcSubfolder)
    {
        var workingDirectory = new TempDirectory();

        await Operations.CopyGitTrackedFilesAsync(
            sourceDirectory: Path.Join(Paths.SrcFolder, srcSubfolder),
            destinationDirectory: workingDirectory.Path);

        return workingDirectory;
    }
}
