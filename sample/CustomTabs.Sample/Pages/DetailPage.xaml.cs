namespace CustomTabs.Sample.Pages;

/// <summary>
/// Simple details page used for navigation demo.
/// </summary>
public partial class DetailPage : ContentPage
{
    /// <summary>
    /// Creates a detail page with the provided title.
    /// </summary>
    public DetailPage(string title)
    {
        InitializeComponent();
        Title = title;
        DetailLabel.Text = title;
    }

    private async void OnPopClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
