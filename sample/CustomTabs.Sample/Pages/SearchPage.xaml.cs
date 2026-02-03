namespace CustomTabs.Sample.Pages;

/// <summary>
/// Search tab root page.
/// </summary>
public partial class SearchPage : ContentPage
{
    /// <summary>
    /// Creates the search page.
    /// </summary>
    public SearchPage()
    {
        InitializeComponent();
    }

    private async void OnPushDetailsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DetailPage("Search details"));
    }
}
