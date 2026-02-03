using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace CustomTabs.Sample;

/// <summary>
/// Application entry point.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Creates the sample app.
    /// </summary>
    public App()
    {
        Debug.WriteLine("[Sample] App constructor invoked.");
        InitializeComponent();
    }
}
