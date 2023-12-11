using System.Windows;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.Messaging;

using Core.RocketLeague.Decryption;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RLUpkSuite.Pages;
using RLUpkSuite.ViewModels;
using RLUpkSuite.Windows;

namespace RLUpkSuite
{
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
            app.MainWindow = host.Services.GetRequiredService<MainWindow>();
            app.MainWindow.Visibility = Visibility.Visible;
            app.Run();

            await host.StopAsync().ConfigureAwait(true);
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder)
                    => configurationBuilder.AddUserSecrets(typeof(App).Assembly))
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<PageBase, DecryptorPageViewModel>();
                    services.AddSingleton<PageBase, CompressorPageViewModel>();
                    services.AddSingleton<PageBase, DummyPackageGeneratorPageViewModel>();
                    services.AddSingleton<PageBase, SettingsPageViewModel>();
                    services.AddTransient<IDecrypterProvider, DecryptionProvider>();
                    services.AddSingleton<UserConfiguration>();

                    services.AddSingleton<WeakReferenceMessenger>();
                    services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider => provider.GetRequiredService<WeakReferenceMessenger>());

                    services.AddSingleton(_ => Current.Dispatcher);

                    services.AddTransient<ISnackbarMessageQueue>(provider =>
                    {
                        Dispatcher dispatcher = provider.GetRequiredService<Dispatcher>();
                        return new SnackbarMessageQueue(TimeSpan.FromSeconds(3.0), dispatcher);
                    });
                });
        }
    }
}