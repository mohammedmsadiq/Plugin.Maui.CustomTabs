using System.Diagnostics;
using CustomTabs.Sample.Services;

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
        try
        {
            base.OnAppearing();
            Debug.WriteLine("[Sample] SplashPage OnAppearing.");
        }
        catch (Exception ex)
        {
            SampleExceptionHandler.Report(ex, "SplashPage.OnAppearing");
        }
    }

    protected override void OnDisappearing()
    {
        try
        {
            base.OnDisappearing();
            Debug.WriteLine("[Sample] SplashPage OnDisappearing.");
        }
        catch (Exception ex)
        {
            SampleExceptionHandler.Report(ex, "SplashPage.OnDisappearing");
        }
    }
}
