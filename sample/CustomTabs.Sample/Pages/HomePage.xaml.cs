using CustomTabs.Sample.Services;

namespace CustomTabs.Sample.Pages;

/// <summary>
/// Home tab root page.
/// </summary>
public partial class HomePage : ContentPage
{
    /// <summary>
    /// Creates the home page.
    /// </summary>
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnPushDetailsClicked(object sender, EventArgs e)
    {
        await SafeExecution.RunAsync(
            () => Navigation.PushAsync(new DetailPage("Home details")),
            "HomePage.OnPushDetailsClicked");
    }
}
