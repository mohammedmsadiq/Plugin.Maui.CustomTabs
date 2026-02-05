using CustomTabs.Sample.Services;
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
        SafeExecution.Run(() => _badge.Count += 1, "MessagesPage.OnIncrementClicked");
    }

    private void OnDecrementClicked(object sender, EventArgs e)
    {
        SafeExecution.Run(() =>
        {
            if (_badge.Count > 0)
            {
                _badge.Count -= 1;
            }
        }, "MessagesPage.OnDecrementClicked");
    }

    private async void OnPushDetailsClicked(object sender, EventArgs e)
    {
        await SafeExecution.RunAsync(
            () => Navigation.PushAsync(new DetailPage("Message details")),
            "MessagesPage.OnPushDetailsClicked");
    }
}
