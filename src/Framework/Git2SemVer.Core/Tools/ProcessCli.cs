using System.Diagnostics;
using System.Runtime.InteropServices;
using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Core.Logging;


namespace NoeticTools.Git2SemVer.Core.Tools;

[RegisterTransient]
public sealed class ProcessCli : IProcessCli
{
    private const int MaxWaitTimeAfterKillMilliseconds = 30000;
    private static readonly object Sync = new();

    public ProcessCli(ILogger logger)
    {
        WorkingDirectory = Environment.CurrentDirectory;
        Logger = logger;
    }

    public ILogger Logger { get; }

    public int TimeLimitMilliseconds { get; set; } = 30000;

    public string WorkingDirectory { get; set; }

    public int Run(string application, string commandLineArguments)
    {
        return Run(application, commandLineArguments, null);
    }

    public int Run(string application, string commandLineArguments, out string standardOutput)
    {
        var outWriter = new StringWriter();
        var errorWriter = new StringWriter();
        var returnCode = Run(application, commandLineArguments, outWriter, errorWriter);
        standardOutput = outWriter.ToString();
        var errorOutput = errorWriter.ToString();
        if (!string.IsNullOrWhiteSpace(errorOutput) && returnCode == 0)
        {
            Logger.LogInfo(standardOutput);
            throw new Git2SemVerArgumentException($"ERROR: {errorOutput}\nOUTPUT:\n{standardOutput}");
        }

        return returnCode;
    }

    /// <summary>
    ///     Run dotnet cli with provided command line arguments.
    /// </summary>
    public int Run(string application, string commandLineArguments,
                   TextWriter? standardOut, TextWriter? errorOut = null)
    {
        lock (Sync)
        {
            Logger.LogTrace($"Running '{application} {commandLineArguments}'.");

            using var process = new Process();
            process.StartInfo.FileName = application;
            process.StartInfo.Arguments = commandLineArguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;

            if (WorkingDirectory.Length > 0)
            {
                process.StartInfo.WorkingDirectory = WorkingDirectory;
            }

            if (standardOut != null)
            {
                process.StartInfo.RedirectStandardOutput = true;
            }

            if (errorOut != null)
            {
                process.StartInfo.RedirectStandardError = true;
                process.ErrorDataReceived += (_, data) => OnErrorDataReceived(data.Data, errorOut);
            }

            process.Start();

            standardOut?.Write(process.StandardOutput.ReadToEnd());

            if (errorOut != null)
            {
                process.BeginErrorReadLine();
            }

            var completed = process.WaitForExit(TimeLimitMilliseconds);
            if (completed)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    process.WaitForExit();
                }
            }

            if (!completed)
            {
                var message =
                    $"ProcessCli Run timed out after {TimeLimitMilliseconds} milliseconds. Command was 'dotnet {commandLineArguments}'.";
                OnError(errorOut, message);
                process.Kill();
                process.WaitForExit(MaxWaitTimeAfterKillMilliseconds);
            }

            var exitCode = process.ExitCode;
            if (exitCode == 0)
            {
                return exitCode;
            }

            Logger.LogInfo(process.StandardOutput.ReadToEnd());
            OnError(errorOut, $"ProcessCli Run returned non-zero exit code {exitCode}.");
            return exitCode;
        }
    }

    public async Task<(int returnCode, string stdOutput)> RunAsync(string application, string commandLineArguments)
    {
        var outWriter = new StringWriter();
        var errorWriter = new StringWriter();
        var returnCode = await RunAsync(application, commandLineArguments, outWriter, errorWriter);
        var output = outWriter.ToString();
        var errorOutput = errorWriter.ToString();
        if (!string.IsNullOrWhiteSpace(errorOutput) && returnCode == 0)
        {
            Logger.LogInfo(output);
            throw new Git2SemVerArgumentException($"ERROR: {errorOutput}\nOUTPUT:\n{output}");
        }

        return (returnCode, output);
    }

    /// <summary>
    ///     Run dotnet cli with provided command line arguments.
    /// </summary>
    public async Task<int> RunAsync(string application, string commandLineArguments,
                                    TextWriter standardOut, TextWriter? errorOut = null)
    {
        //        Logger.LogTrace($"Running '{application} {commandLineArguments}'.");
        Logger.LogInfo($"Running '{application} {commandLineArguments}'.");

        using var process = new Process();
        process.StartInfo.FileName = application;
        process.StartInfo.Arguments = commandLineArguments;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        if (WorkingDirectory.Length > 0)
        {
            process.StartInfo.WorkingDirectory = WorkingDirectory;
        }

        if (errorOut != null)
        {
            process.ErrorDataReceived += (_, data) => OnErrorDataReceived(data.Data, errorOut);
        }

        process.Start();

        process.BeginErrorReadLine();

        var standardOutput = await process.StandardOutput.ReadToEndAsync();
        await standardOut.WriteAsync(standardOutput);

        var completed = process.WaitForExit(TimeLimitMilliseconds);
        if (completed)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // ReSharper disable once MethodHasAsyncOverload
                process.WaitForExit();
            }
        }

        if (!completed)
        {
            var message =
                $"ProcessCli Run timed out after {TimeLimitMilliseconds} milliseconds. Command was 'dotnet {commandLineArguments}'.";
            OnError(errorOut, message);
            process.Kill();
            process.WaitForExit(MaxWaitTimeAfterKillMilliseconds);
        }

        var exitCode = process.ExitCode;
        if (exitCode != 0)
        {
            var message = $"ProcessCli Run returned non-zero exit code {exitCode}.";
            OnError(errorOut, message);
        }

        await standardOut.FlushAsync();

        return exitCode;
    }

    private void OnError(TextWriter? errorOut, string message)
    {
        errorOut?.WriteLine(message);
        if (errorOut == null)
        {
            Logger.LogError(message);
        }
    }

    private static void OnErrorDataReceived(string? data, TextWriter errorOut)
    {
        if (data == null)
        {
            return;
        }

        errorOut.WriteLine(data);
    }
}