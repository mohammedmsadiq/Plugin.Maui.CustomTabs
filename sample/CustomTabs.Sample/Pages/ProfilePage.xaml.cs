namespace CustomTabs.Sample.Pages;

/// <summary>
/// Profile tab root page.
/// </summary>
public partial class ProfilePage : ContentPage
{
    /// <summary>
    /// Creates the profile page.
    /// </summary>
    public ProfilePage()
    {
        InitializeComponent();
    }

    private async void OnPushDetailsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DetailPage("Profile details"));
    }
}
