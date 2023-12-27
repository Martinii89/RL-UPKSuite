using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

using CommunityToolkit.Mvvm.Input;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.Logging;

using RLUpkSuite.Config;
using RLUpkSuite.Pages;
using RLUpkSuite.ViewModels;

namespace RLUpkSuite.PackageConversion;

public partial class PackageGeneratorPageViewModel : PageBase
{
    private readonly ConversionConfig _conversionConfig;

    private readonly ILogger<PackageGeneratorPageViewModel> _logger;

    private readonly PackageConverterFactory _packageConverterFactory;

    public PackageGeneratorPageViewModel(ConversionConfig conversionConfig,
        PackageConverterFactory packageConverterFactory,
        ILogger<PackageGeneratorPageViewModel> logger)
        : base("Generate", PackIconKind.Pencil)
    {
        _conversionConfig = conversionConfig;
        _packageConverterFactory = packageConverterFactory;
        _logger = logger;
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

    public string? ImportPackagesDirectory
    {
        get => _conversionConfig.ImportPackagesDirectory;
        set
        {
            if (SetProperty(_conversionConfig.ImportPackagesDirectory, value, _conversionConfig,
                    (config, s) => config.ImportPackagesDirectory = s))
            {
                ConvertFilesCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string? Suffix
    {
        get => _conversionConfig.Suffix;
        set => SetProperty(_conversionConfig.Suffix, value, _conversionConfig,
            (config, b) => config.Suffix = b);
    }

    public bool Compress
    {
        get => _conversionConfig.Compress;
        set => SetProperty(_conversionConfig.Compress, value, _conversionConfig,
            (config, b) => config.Compress = b);
    }

    public int Threads
    {
        get => _conversionConfig.Threads;
        set => SetProperty(_conversionConfig.Threads, value, _conversionConfig,
            (config, b) => config.Threads = b);
    }

    public bool OpenOutputDirectoryOnFinished
    {
        get => _conversionConfig.OpenOutputOnFinish;
        set => SetProperty(_conversionConfig.OpenOutputOnFinish, value, _conversionConfig,
            (config, b) => config.OpenOutputOnFinish = b);
    }

    [RelayCommand]
    private async Task AddFiles(IEnumerable<string> paths)
    {
        List<string> validFiles = [];
        await Task.Run(() => { validFiles.AddRange(paths.Where(Path.Exists)); });
        foreach (string validFile in validFiles)
        {
            FileReferences.Add(new FileReference(validFile));
        }
    }

    [RelayCommand]
    private async Task ConvertFiles()
    {
        await CovertFilesImpl();
        if (_conversionConfig.OpenOutputOnFinish && OutputDirectory != null)
        {
            Process.Start("explorer.exe", OutputDirectory);
        }
    }

    private async Task CovertFilesImpl()
    {
        if (_conversionConfig.KeysPath is null || _conversionConfig.ImportPackagesDirectory is null)
        {
            return;
        }

        ConcurrentQueue<FileReference> processQueue = new(FileReferences);
        Stopwatch sw = Stopwatch.StartNew();

        IEnumerable<Task> processTasks = Enumerable.Range(0, _conversionConfig.Threads).Select(i =>
        {
            return Task.Run(() =>
            {
                PackageConverter? converter = _packageConverterFactory.Create(_conversionConfig);
                while (processQueue.TryDequeue(out FileReference? file))
                {
                    converter!.ProcessFile(file);
                }
            });
        });
        await Task.WhenAll(processTasks);
        sw.Stop();
        _logger.LogInformation("Processed {FileCount} files in {Duration} s", FileReferences.Count,
            sw.Elapsed.TotalSeconds);
    }

    public static IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> list, int parts)
    {
        int i = 0;
        IEnumerable<IEnumerable<T>> splits = list.GroupBy(item => i++ % parts).Select(part => part.AsEnumerable());
        return splits;
    }
}