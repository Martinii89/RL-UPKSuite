using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RlUpk.RLUpkSuite.AppUpdates;

public partial class UpdateDialogViewModel : ObservableObject
{
    private readonly UpdateHelper _updateHelper;

    [ObservableProperty]
    private int _progress;

    public UpdateDialogViewModel(UpdateHelper updateHelper)
    {
        _updateHelper = updateHelper;
    }

    [RelayCommand]
    private async Task StartUpdate()
    {
        await _updateHelper.StartUpdate(UpdateProgress);
    }

    private void UpdateProgress(int p)
    {
        Application.Current.Dispatcher.BeginInvoke(() => { Progress = p; });
    }
}