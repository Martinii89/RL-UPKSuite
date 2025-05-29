using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using MaterialDesignThemes.Wpf;

using RlUpk.RLUpkSuite.Config;
using RlUpk.RLUpkSuite.Pages;

namespace RlUpk.RLUpkSuite.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IRecipient<ShowError>, IRecipient<MessageWithDialogDetails>
{
    private const string TitleBase = "RlUpkSuite";

    private readonly ShellConfig _shellConfig;

    [ObservableProperty]
    private bool _isNavigatorOpen;

    [ObservableProperty]
    private PageBase? _selectedPage;

    [ObservableProperty]
    private string _title = TitleBase;

    /// <inheritdoc />
    public MainWindowViewModel(
        IEnumerable<PageBase> pages,
        ShellConfig shellConfig,
        ISnackbarMessageQueue messageQueue,
        IMessenger messenger)
    {
        _shellConfig = shellConfig;
        Pages = [..pages];

        MessageQueue = messageQueue;
        messenger.Register<ShowError>(this);
        messenger.Register<MessageWithDialogDetails>(this);
    }

    public ObservableCollection<PageBase> Pages { get; }

    public ISnackbarMessageQueue MessageQueue { get; }

    public void Receive(MessageWithDialogDetails message)
    {
        MessageQueue.Enqueue(message.Message, message.ActionMessage, OnShowDialogDetails, message, false, false,
            message.DurationOverride);
    }

    public void Receive(ShowError message)
    {
        MessageQueue.Enqueue(message.Message, "Details", OnShowErrorDetails, message.Details);
    }

    partial void OnSelectedPageChanged(PageBase? value)
    {
        IsNavigatorOpen = false;
        if (value is null)
        {
            return;
        }

        Title = $"{TitleBase} - {value.PageName}";
        _shellConfig.StartPage = value.PageName;
    }

    private static async void OnShowErrorDetails(string? details)
    {
        if (string.IsNullOrEmpty(details))
        {
            return;
        }

        await DialogHost.Show(new ErrorDetailsViewModel(details), "Root");
    }

    private static async void OnShowDialogDetails(MessageWithDialogDetails? dialogContent)
    {
        if (dialogContent != null)
        {
            await DialogHost.Show(dialogContent, "Root");
        }
    }
}

public record class ShowError(string Message, string Details);

public record class MessageWithDialogDetails(
    string Message,
    string ActionMessage,
    object DialogContent,
    TimeSpan? DurationOverride);

public record class ErrorDetailsViewModel(string Details);