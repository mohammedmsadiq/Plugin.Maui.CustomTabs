using CustomTabs.Prism.Sample.Pages;
using CustomTabs.Prism.Sample.Services;
using Microsoft.Maui.Controls;
using Prism.Ioc;

namespace CustomTabs.Prism.Sample;

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
