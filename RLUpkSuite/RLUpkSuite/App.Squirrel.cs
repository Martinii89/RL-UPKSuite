using System.Windows;

using RlUpk.RLUpkSuite.AppUpdates;

using Squirrel;

namespace RlUpk.RLUpkSuite;

public partial class App
{
    public static SemanticVersion? Version { get; private set; }

    private UpdateHelper? UpdateHelper { get; set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        SquirrelAwareApp.HandleEvents(
            OnAppInstall,
            onAppUninstall: OnAppUninstall,
            onEveryRun: OnAppRun);


        if (UpdateHelper != null)
        {
            await UpdateHelper.CheckUpdate();
        }
    }

    private static void OnAppInstall(SemanticVersion version, IAppTools tools)
    {
        tools.CreateShortcutForThisExe();
    }

    private static void OnAppRun(SemanticVersion version, IAppTools tools, bool firstRun)
    {
        Version = version;
        tools.SetProcessAppUserModelId();
    }

    private static void OnAppUninstall(SemanticVersion version, IAppTools tools)
    {
        tools.RemoveShortcutForThisExe();
    }
}