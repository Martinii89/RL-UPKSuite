using System.IO;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Core.RocketLeague.Decryption;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RLUpkSuite.Config;
using RLUpkSuite.Pages;
using RLUpkSuite.ViewModels;
using RLUpkSuite.Windows;

namespace RLUpkSuite;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    [STAThread]
    private static void Main(string[] args)
    {
        MainAsync(args).GetAwaiter().GetResult();
    }

    private static async Task MainAsync(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();
        await host.StartAsync().ConfigureAwait(true);

        App app = new();
        app.InitializeComponent();
        var appMainWindow = host.Services.GetRequiredService<MainWindow>();
        appMainWindow.InitConfig(GetAppConfigPath());
        app.MainWindow = appMainWindow;
        app.MainWindow.Visibility = Visibility.Visible;
        app.Run();

        await host.StopAsync().ConfigureAwait(true);
    }

    private static string GetAppConfigPath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "rlupksuite",
            "rlupksuite.config");
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder)
                => configurationBuilder.AddUserSecrets(typeof(App).Assembly))
            .ConfigureServices((hostContext, services) =>
            {
                // Plumbing
                services.AddSingleton<WeakReferenceMessenger>();
                services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider =>
                    provider.GetRequiredService<WeakReferenceMessenger>());
                services.AddSingleton(_ => Current.Dispatcher);
                services.AddTransient<ISnackbarMessageQueue>(provider =>
                {
                    var dispatcher = provider.GetRequiredService<Dispatcher>();
                    return new SnackbarMessageQueue(TimeSpan.FromSeconds(3.0), dispatcher);
                });

                //Windows and pages
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddSingleton<PageBase, DecryptorPageViewModel>();
                services.AddSingleton<PageBase, CompressorPageViewModel>();
                services.AddSingleton<PageBase, DummyPackageGeneratorPageViewModel>();
                services.AddSingleton<PageBase, SettingsPageViewModel>();

                //Config
                services.AddSingleton<AppConfigStore>();
                services.AddAppConfig<ShellConfig>();
                services.AddAppConfig<DecryptionConfig>();
                services.AddAppConfig<CommonConfig>();

                //RL upk suite stuff
                services.AddTransient<IDecrypterProvider, DecryptionProvider>();
            });
    }
}