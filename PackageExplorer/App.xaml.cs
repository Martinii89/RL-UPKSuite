using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Syncfusion.Licensing;

namespace PackageExplorer;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            //.AddJsonFile("appsettings.json")
            .AddUserSecrets<App>()
            .Build();
        var key = config["SyncfusionLiceneseKey"];
        SyncfusionLicenseProvider.RegisterLicense(key);
    }
}