using System.Diagnostics;

namespace CustomTabs.Sample.Pages;

/// <summary>
/// Splash page that redirects based on login status.
/// </summary>
public partial class SplashPage : ContentPage
{
    /// <summary>
    /// Creates the splash page.
    /// </summary>
    public SplashPage()
    {
        Debug.WriteLine("[Sample] SplashPage constructor invoked.");
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("[Sample] SplashPage OnAppearing.");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Debug.WriteLine("[Sample] SplashPage OnDisappearing.");
    }
}
