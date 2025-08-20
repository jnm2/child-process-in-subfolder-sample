using System;
using MainApp;
using System.Runtime.InteropServices;

using var childProcessClient = new ChildProcessClient();

Console.WriteLine($"Received from child process: \"{childProcessClient.StandardOutput.ReadLine()}\"");

childProcessClient.StandardInput.WriteLine($"Hello from {RuntimeInformation.FrameworkDescription}!");

Console.WriteLine($"Received from child process: \"{childProcessClient.StandardOutput.ReadLine()}\"");
