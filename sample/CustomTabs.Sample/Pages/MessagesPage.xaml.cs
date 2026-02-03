using Plugin.Maui.CustomTabs.Models;

namespace CustomTabs.Sample.Pages;

/// <summary>
/// Messages tab root page.
/// </summary>
public partial class MessagesPage : ContentPage
{
    private readonly CustomTabBadge _badge;

    /// <summary>
    /// Creates the messages page.
    /// </summary>
    public MessagesPage(CustomTabBadge badge)
    {
        _badge = badge;
        InitializeComponent();
        BindingContext = _badge;
    }

    private void OnIncrementClicked(object sender, EventArgs e)
    {
        _badge.Count += 1;
    }

    private void OnDecrementClicked(object sender, EventArgs e)
    {
        if (_badge.Count > 0)
        {
            _badge.Count -= 1;
        }
    }

    private async void OnPushDetailsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DetailPage("Message details"));
    }
}
