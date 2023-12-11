using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using MaterialDesignThemes.Wpf;

using RLUpkSuite.Pages;

namespace RLUpkSuite.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject, IRecipient<ShowError>
    {
        private const string TitleBase = "RlUpkSuite";
        private readonly UserConfiguration _userConfiguration;

        [ObservableProperty]
        private bool _isNavigatorOpen;

        [ObservableProperty]
        private PageBase? _selectedPage;

        [ObservableProperty]
        private string _title = TitleBase;

        /// <inheritdoc />
        public MainWindowViewModel(
            UserConfiguration userConfiguration,
            IEnumerable<PageBase> pages,
            ISnackbarMessageQueue messageQueue,
            IMessenger messenger)
        {
            _userConfiguration = userConfiguration;
            messenger.Register(this);
            MessageQueue = messageQueue;
            Pages = [..pages];
            SelectedPage = Pages.FirstOrDefault(x => x.PageName == _userConfiguration.StartPage) ?? Pages.FirstOrDefault();
        }

        public ObservableCollection<PageBase> Pages { get; }

        public ISnackbarMessageQueue MessageQueue { get; }

        public void Receive(ShowError message)
        {
            MessageQueue.Enqueue(message.Message, "Details", OnShowErrorDetails, message.Details);
        }

        public void SaveConfig()
        {
            _userConfiguration.Save();
        }

        partial void OnSelectedPageChanged(PageBase? value)
        {
            IsNavigatorOpen = false;
            if (value is null)
            {
                return;
            }

            Title = $"{TitleBase} - {value.PageName}";
            _userConfiguration.StartPage = value.PageName;
        }

        private async void OnShowErrorDetails(string? details)
        {
            if (string.IsNullOrEmpty(details))
            {
                return;
            }

            await DialogHost.Show(new ErrorDetailsViewModel(details), "Root");
        }
    }

    public record class ShowError(string Message, string Details);

    public record class ErrorDetailsViewModel(string Details);
}