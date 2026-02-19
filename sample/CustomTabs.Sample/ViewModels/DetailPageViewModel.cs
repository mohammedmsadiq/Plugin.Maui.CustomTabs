using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CustomTabs.Sample.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace CustomTabs.Sample.ViewModels;

/// <summary>
/// View model for the detail page.
/// </summary>
public sealed class DetailPageViewModel : BindableBase, IInitialize
{
    private readonly INavigationService _navigationService;
    private string _title = "Details";

    /// <summary>
    /// Title displayed on the details screen.
    /// </summary>
    public string Title
    {
        get => _title;
        private set => SetProperty(ref _title, value);
    }

    /// <summary>
    /// Command to navigate back.
    /// </summary>
    public DelegateCommand GoBackCommand { get; }

    /// <summary>
    /// Creates the detail view model.
    /// </summary>
    public DetailPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        GoBackCommand = new DelegateCommand(async () =>
            await SafeExecution.RunAsync(
                () => _navigationService.GoBackAsync(),
                "DetailPageViewModel.GoBackCommand"));
    }

    /// <inheritdoc />
    public void Initialize(INavigationParameters parameters)
    {
        if (TryGetParameter(parameters, "title", out var value) && !string.IsNullOrWhiteSpace(value))
        {
            Title = value;
        }
    }

    private static bool TryGetParameter(INavigationParameters parameters, string key, out string? value)
    {
        value = null;
        if (parameters is null || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        if (parameters is IDictionary<string, object> dict && dict.TryGetValue(key, out var dictValue))
        {
            value = dictValue as string;
            return value != null;
        }

        if (parameters is IReadOnlyDictionary<string, object> roDict && roDict.TryGetValue(key, out var roValue))
        {
            value = roValue as string;
            return value != null;
        }

        var type = parameters.GetType();
        var indexer = type.GetProperty("Item", new[] { typeof(string) });
        if (indexer != null)
        {
            var indexedValue = indexer.GetValue(parameters, new object[] { key });
            value = indexedValue as string;
            if (value != null)
            {
                return true;
            }
        }

        var getValue = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(method => method.Name == "GetValue"
                                      && method.IsGenericMethodDefinition
                                      && method.GetParameters().Length == 1
                                      && method.GetParameters()[0].ParameterType == typeof(string));
        if (getValue != null)
        {
            var generic = getValue.MakeGenericMethod(typeof(string));
            var result = generic.Invoke(parameters, new object[] { key });
            value = result as string;
            return value != null;
        }

        return false;
    }
}
