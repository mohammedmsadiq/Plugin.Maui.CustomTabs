using System.Diagnostics;
using CustomTabs.Sample.Pages;
using CustomTabs.Sample.Services;
using Microsoft.Maui.Controls;
using Prism.Ioc;

namespace CustomTabs.Sample;

/// <summary>
/// Application entry point.
/// </summary>
public partial class App : Application
{
    private readonly IContainerProvider _container;

    /// <summary>
    /// Creates the sample app.
    /// </summary>
    public App(IContainerProvider container)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
        Debug.WriteLine("[Sample] App constructor invoked.");
        SampleExceptionHandler.RegisterGlobalHandlers();
        InitializeComponent();
    }

    /// <inheritdoc />
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var page = _container.Resolve<MainTabsPage>();
        return new Window(page);
    }
}
