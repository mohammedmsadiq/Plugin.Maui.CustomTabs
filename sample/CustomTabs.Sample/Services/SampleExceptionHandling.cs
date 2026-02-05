using System.Diagnostics;
using System.Threading;
using Plugin.Maui.CustomTabs.Services;

namespace CustomTabs.Sample.Services;

/// <summary>
/// Centralized exception handling for the sample app.
/// </summary>
public sealed class SampleExceptionHandler : ICustomTabsExceptionHandler
{
    private static int _handlersRegistered;

    /// <inheritdoc />
    public void Handle(Exception exception, string context)
    {
        Report(exception, context);
    }

    /// <summary>
    /// Registers global exception handlers once.
    /// </summary>
    public static void RegisterGlobalHandlers()
    {
        if (Interlocked.Exchange(ref _handlersRegistered, 1) == 1)
        {
            return;
        }

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception exception)
            {
                Report(exception, "AppDomain.CurrentDomain.UnhandledException");
            }
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            Report(args.Exception, "TaskScheduler.UnobservedTaskException");
            args.SetObserved();
        };
    }

    /// <summary>
    /// Reports an exception to debug output.
    /// </summary>
    public static void Report(Exception exception, string context)
    {
        Debug.WriteLine($"[Sample][Error] {context}: {exception}");
    }
}

/// <summary>
/// Helper for wrapping sample actions in try/catch.
/// </summary>
public static class SafeExecution
{
    /// <summary>
    /// Executes an action with exception handling.
    /// </summary>
    public static void Run(Action action, string context)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            SampleExceptionHandler.Report(ex, context);
        }
    }

    /// <summary>
    /// Executes an async action with exception handling.
    /// </summary>
    public static async Task RunAsync(Func<Task> action, string context)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            SampleExceptionHandler.Report(ex, context);
        }
    }
}
