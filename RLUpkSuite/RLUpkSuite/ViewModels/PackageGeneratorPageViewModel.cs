using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;

using MaterialDesignThemes.Wpf;

using RLUpkSuite.Config;
using RLUpkSuite.Pages;

namespace RLUpkSuite.ViewModels;

public partial class PackageGeneratorPageViewModel : PageBase
{
    private readonly ConversionConfig _conversionConfig;

    public PackageGeneratorPageViewModel(ConversionConfig conversionConfig) : base("Generate", PackIconKind.Pencil)
    {
        _conversionConfig = conversionConfig;
    }

    public FileReferenceCollection FileReferences { get; } = [];
    
    public string? KeyPath
    {
        get => _conversionConfig.KeysPath;
        set
        {
            if (SetProperty(_conversionConfig.KeysPath, value, _conversionConfig, (config, s) => config.KeysPath = s))
            {
                ConvertFilesCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string? OutputDirectory
    {
        get => _conversionConfig.OutputDirectory;
        set
        {
            if (SetProperty(_conversionConfig.OutputDirectory, value, _conversionConfig,
                    (config, s) => config.OutputDirectory = s))
            {
                ConvertFilesCommand.NotifyCanExecuteChanged();
            }
        }
    }

    [RelayCommand]
    private Task ConvertFiles()
    {
        return Task.CompletedTask;
    }
}