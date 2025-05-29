using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RlUpk.RLUpkSuite.AppSettings;
using RlUpk.RLUpkSuite.ViewModels;

using Squirrel;
using Squirrel.Sources;

namespace RlUpk.RLUpkSuite.AppUpdates;

public class UpdateHelper
{
    private readonly Deployment _deployment;

    private readonly ILogger<UpdateHelper> _logger;

    private readonly IMessenger _messenger;

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

        using UpdateManager? mgr = CreateUpdateManager();
        if (mgr is null)
        {
            _logger.LogInformation("UpdateManager null");
            return;
        }

        UpdateInfo? updateInfo = await mgr.CheckForUpdate().ConfigureAwait(false);
        if (updateInfo is null)
        {
            return;
        }

        List<ReleaseEntry>? updates = updateInfo.ReleasesToApply;
        if (updates is null || updates.Count == 0)
        {
            _logger.LogInformation("No new updates");
            return;
        }


        SemanticVersion? currentVersion = mgr.CurrentlyInstalledVersion();
        SemanticVersion? futureVersion = updateInfo.FutureReleaseEntry.Version;
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
        _messenger.Send(new MessageWithDialogDetails("New version available", "Update", new UpdateDialogViewModel(this),
            TimeSpan.FromSeconds(10)));
    }

    public async Task StartUpdate(Action<int> updateReporting)
    {
        using UpdateManager? mgr = CreateUpdateManager();
        if (mgr is null)
        {
            _logger.LogInformation("UpdateManager null");
            return;
        }

        ReleaseEntry? newVersion = await mgr.UpdateApp(updateReporting);

        // optionally restart the app automatically, or ask the user if/when they want to restart
        if (newVersion != null)
        {
            UpdateManager.RestartApp();
        }
    }
}