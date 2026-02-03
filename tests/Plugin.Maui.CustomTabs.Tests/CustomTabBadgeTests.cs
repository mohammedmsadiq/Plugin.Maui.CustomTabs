#if ANDROID || IOS
using NUnit.Framework;
using Plugin.Maui.CustomTabs.Models;

namespace Plugin.Maui.CustomTabs.Tests;

[TestFixture]
public sealed class CustomTabBadgeTests
{
    [Test]
    public void DotBadge_IsVisible_WhenDotEnabled()
    {
        var badge = new CustomTabBadge { IsDot = true };

        Assert.That(badge.IsVisible, Is.True);
        Assert.That(badge.IsDotVisible, Is.True);
        Assert.That(badge.IsCountVisible, Is.False);
    }

    [Test]
    public void CountBadge_UsesCount_WhenPositive()
    {
        var badge = new CustomTabBadge { Count = 5 };

        Assert.That(badge.IsVisible, Is.True);
        Assert.That(badge.IsCountVisible, Is.True);
        Assert.That(badge.DisplayText, Is.EqualTo("5"));
    }

    [Test]
    public void CountBadge_UsesMaxCountSuffix_WhenExceeded()
    {
        var badge = new CustomTabBadge { Count = 120, MaxCount = 99 };

        Assert.That(badge.DisplayText, Is.EqualTo("99+"));
    }

    [Test]
    public void CountBadge_UsesTextOverride()
    {
        var badge = new CustomTabBadge { Text = "New" };

        Assert.That(badge.IsVisible, Is.True);
        Assert.That(badge.DisplayText, Is.EqualTo("New"));
    }
}
#endif
