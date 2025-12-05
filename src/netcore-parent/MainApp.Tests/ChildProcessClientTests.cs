using NUnit.Framework;
using Shouldly;

namespace MainApp.Tests;

public class ChildProcessClientTests
{
    [Test]
    public void Verify_child_process_behavior()
    {
        // Demonstrates that the child process subfolder is transitively copied to the output of the test project.
        using var client = new ChildProcessClient();

        client.StandardOutput.ReadLine().ShouldStartWith("Hello from .NET Framework 4.8");

        client.StandardInput.WriteLine("String from Verify_child_process_behavior");
        client.StandardOutput.ReadLine().ShouldBe("""Received "String from Verify_child_process_behavior" from the parent process""");
    }
}
