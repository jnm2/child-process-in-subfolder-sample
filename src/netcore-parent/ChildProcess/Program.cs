using System;
using System.Runtime.InteropServices;

public static class Program
{
    public static int Main()
    {
        if (!Console.IsInputRedirected || !Console.IsOutputRedirected)
        {
            Console.WriteLine("This program is intended to be run as a child process with redirected input and output.");
            return 1;
        }

        Console.WriteLine($"Hello from {RuntimeInformation.FrameworkDescription}!");

        while (true)
        {
            var line = Console.ReadLine();
            if (line is null)
            {
                // The main process has either exited or otherwise has closed the input stream.
                // Time to exit so that the child process doesn't hang around.
                break;
            }

            Console.WriteLine($"Received \"{line}\" from the parent process");
        }

        return 0;
    }
}
