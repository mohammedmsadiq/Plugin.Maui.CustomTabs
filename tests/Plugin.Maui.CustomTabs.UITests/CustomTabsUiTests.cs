using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;

namespace Plugin.Maui.CustomTabs.UITests;

[TestFixture]
public sealed class CustomTabsUiTests
{
    private AppiumDriver<AppiumWebElement>? _driver;

    [SetUp]
    public void SetUp()
    {
        _driver = CreateDriver();
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            _driver?.Quit();
        }
        catch
        {
            // Ignore driver shutdown failures.
        }
    }

    [Test]
    public void Tabs_PreserveStack_AndReselectPopsToRoot()
    {
        var driver = RequireDriver();

        WaitForElement(driver, "tab-home");
        Tap(driver, "home-push-details");
        WaitForElement(driver, "detail-title");

        Tap(driver, "tab-search");
        WaitForElement(driver, "search-title");

        Tap(driver, "tab-home");
        WaitForElement(driver, "detail-title");

        Tap(driver, "tab-home");
        WaitForElement(driver, "home-title");
    }

    private AppiumDriver<AppiumWebElement> RequireDriver()
    {
        if (_driver == null)
        {
            throw new InvalidOperationException("Appium driver not initialized.");
        }

        return _driver;
    }

    private static AppiumDriver<AppiumWebElement> CreateDriver()
    {
        var appPath = Environment.GetEnvironmentVariable("MAUI_APP_PATH");
        if (string.IsNullOrWhiteSpace(appPath))
        {
            Assert.Inconclusive("Set MAUI_APP_PATH to the built app package (apk/app/ipa).");
        }

        var platform = Environment.GetEnvironmentVariable("MAUI_PLATFORM");
        if (string.IsNullOrWhiteSpace(platform))
        {
            platform = InferPlatform(appPath!);
        }

        if (string.IsNullOrWhiteSpace(platform))
        {
            Assert.Inconclusive("Set MAUI_PLATFORM to Android or iOS.");
        }

        var serverUrl = Environment.GetEnvironmentVariable("APPIUM_SERVER_URL") ?? "http://127.0.0.1:4723/";
        var options = new AppiumOptions();
        options.AddAdditionalCapability("app", appPath);
        options.AddAdditionalCapability("platformName", platform);

        if (platform.Equals("Android", StringComparison.OrdinalIgnoreCase))
        {
            options.AddAdditionalCapability("automationName", Environment.GetEnvironmentVariable("APPIUM_AUTOMATION") ?? "UiAutomator2");
            options.AddAdditionalCapability("deviceName", Environment.GetEnvironmentVariable("APPIUM_DEVICE_NAME") ?? "Android Emulator");
            return new AndroidDriver<AppiumWebElement>(new Uri(serverUrl), options);
        }

        if (platform.Equals("iOS", StringComparison.OrdinalIgnoreCase))
        {
            options.AddAdditionalCapability("automationName", Environment.GetEnvironmentVariable("APPIUM_AUTOMATION") ?? "XCUITest");
            options.AddAdditionalCapability("deviceName", Environment.GetEnvironmentVariable("APPIUM_DEVICE_NAME") ?? "iPhone Simulator");
            return new IOSDriver<AppiumWebElement>(new Uri(serverUrl), options);
        }

        Assert.Inconclusive("Unsupported platform for UI tests.");
        return null!;
    }

    private static string InferPlatform(string appPath)
    {
        var extension = Path.GetExtension(appPath).ToLowerInvariant();
        return extension switch
        {
            ".apk" => "Android",
            ".app" => "iOS",
            ".ipa" => "iOS",
            _ => string.Empty
        };
    }

    private static void Tap(AppiumDriver<AppiumWebElement> driver, string accessibilityId)
    {
        var element = WaitForElement(driver, accessibilityId);
        element.Click();
    }

    private static AppiumWebElement WaitForElement(AppiumDriver<AppiumWebElement> driver, string accessibilityId)
    {
        var timeout = TimeSpan.FromSeconds(20);
        var stopAt = DateTime.UtcNow + timeout;

        while (DateTime.UtcNow < stopAt)
        {
            try
            {
                var element = driver.FindElementByAccessibilityId(accessibilityId);
                if (element.Displayed)
                {
                    return element;
                }
            }
            catch (NoSuchElementException)
            {
            }

            Thread.Sleep(250);
        }

        Assert.Fail($"Element '{accessibilityId}' not found within timeout.");
        return null!;
    }
}
