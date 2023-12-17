using System.IO;

using CommunityToolkit.Mvvm.Messaging;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RLUpkSuite.AppSettings;
using RLUpkSuite.ViewModels;

using Squirrel;
using Squirrel.Sources;

namespace RLUpkSuite.AppUpdates;

public class UpdateHelper
{
    private readonly ILogger<UpdateHelper> _logger;

    private readonly IMessenger _messenger;

    private readonly Deployment _deployment;

    public UpdateHelper(ILogger<UpdateHelper> logger, IMessenger messenger, IOptions<Deployment> deploymentOptions)
    {
        _logger = logger;
        _messenger = messenger;
        _deployment = deploymentOptions.Value;
    }

    private UpdateManager? CreateUpdateManager()
    {
        UpdateManager updateManager = _deployment.SourceType switch
        {
            "github" => new UpdateManager(new GithubSource(_deployment.Source, null, false)),
            _ => new UpdateManager(_deployment.Source)
        };

        return updateManager.IsInstalledApp ? updateManager : null;
    }

    public async Task CheckUpdate()
    {
        _logger.LogInformation("Checking for update");

        using var mgr = CreateUpdateManager();
        if (mgr is null)
        {
            _logger.LogInformation("UpdateManager null");
            return;
        }

        var updateInfo = await mgr.CheckForUpdate().ConfigureAwait(false);
        if (updateInfo is null)
        {
            return;
        }
        var updates = updateInfo.ReleasesToApply;
        if (updates is null || updates.Count == 0)
        {
            _logger.LogInformation("No new updates");
            return;
        }

        
        var currentVersion = mgr.CurrentlyInstalledVersion();
        var futureVersion = updateInfo.FutureReleaseEntry.Version;
        if (currentVersion != null && currentVersion >= futureVersion)
        {
            _logger.LogInformation("No new updates");
            return;
        }
        

        _logger.LogInformation("New version available: {CurrentVersion} -> {NewVersion}",
            currentVersion, futureVersion);
        SendUpdateNotification();
    }

    private void SendUpdateNotification()
    {
        _messenger.Send(new MessageWithDialogDetails("New version available", "Update", new UpdateDialogViewModel(this), TimeSpan.FromSeconds(10)));
    }

    public async Task StartUpdate(Action<int> updateReporting)
    {
        using var mgr = CreateUpdateManager();
        if (mgr is null)
        {
            _logger.LogInformation("UpdateManager null");
            return;
        }
        var newVersion = await mgr.UpdateApp(updateReporting);
   
        // optionally restart the app automatically, or ask the user if/when they want to restart
        if (newVersion != null) {
            UpdateManager.RestartApp();
        }
    }
}