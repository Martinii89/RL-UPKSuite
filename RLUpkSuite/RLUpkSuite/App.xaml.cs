using System.IO;
using System.Windows;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.Messaging;

using Core.RocketLeague.Decryption;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using RLUpkSuite.AppSettings;
using RLUpkSuite.AppUpdates;
using RLUpkSuite.Config;
using RLUpkSuite.PackageConversion;
using RLUpkSuite.Pages;
using RLUpkSuite.ViewModels;
using RLUpkSuite.Windows;

using PackageGeneratorPageViewModel = RLUpkSuite.PackageConversion.PackageGeneratorPageViewModel;

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
        using IHost host = CreateHostBuilder(args).Build();
        await host.StartAsync().ConfigureAwait(true);

        App app = new();
        app.InitializeComponent();
        MainWindow appMainWindow = host.Services.GetRequiredService<MainWindow>();
        app.UpdateHelper = host.Services.GetService<UpdateHelper>();
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
            .ConfigureAppConfiguration((context, configurationBuilder)
                =>
            {
                configurationBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);
#if DEBUG
                configurationBuilder.AddJsonFile("appsettings.Development.json");
#endif
                // configurationBuilder.AddUserSecrets(typeof(App).Assembly);
            })
            .ConfigureServices((hostContext, services) =>
            {
                // Plumbing
                services.AddSingleton<WeakReferenceMessenger>();
                services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider =>
                    provider.GetRequiredService<WeakReferenceMessenger>());
                services.AddSingleton(_ => Current.Dispatcher);
                services.AddTransient<ISnackbarMessageQueue>(provider =>
                {
                    Dispatcher dispatcher = provider.GetRequiredService<Dispatcher>();
                    return new SnackbarMessageQueue(TimeSpan.FromSeconds(3.0), dispatcher);
                });
                services.AddSingleton<UpdateHelper>();


                services.AddOptions<Deployment>()
                    .BindConfiguration(Deployment.Section)
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

                //Windows and pages
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddSingleton<PageBase, DecryptorPageViewModel>();
                services.AddSingleton<PageBase, CompressorPageViewModel>();
                services.AddSingleton<PageBase, PackageGeneratorPageViewModel>();
                services.AddSingleton<PageBase, SettingsPageViewModel>();

                //Config
                services.AddSingleton<AppConfigStore>();
                services.AddAppConfig<ShellConfig>();
                services.AddAppConfig<DecryptionConfig>();
                services.AddAppConfig<CommonConfig>();
                services.AddAppConfig<ConversionConfig>();


                //RL upk suite stuff
                services.TryAddTransient<IDecrypterProvider, DecryptionProvider>();
                services.AddPackageConversion();
            });
    }
}