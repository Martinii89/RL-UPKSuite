using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

using CommunityToolkit.Mvvm.Input;

using Core;
using Core.Classes.Compression;
using Core.RocketLeague.Decryption;
using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Serialization.Default;
using Core.Serialization.RocketLeague;
using Core.Types;
using Core.Utility;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using RLUpkSuite.Config;
using RLUpkSuite.Pages;
using RLUpkSuite.ViewModels;

namespace RLUpkSuite.PackageConversion;

public partial class PackageGeneratorPageViewModel : PageBase
{
    private readonly ConversionConfig _conversionConfig;

    private readonly PackageConverterFactory _packageConverterFactory;

    private readonly ILogger<PackageGeneratorPageViewModel> _logger;

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

        // var rlServices = _rlSerializerCollection;
        // var udkServices = _udkSerializerCollection;
        // var nativeFactory = new NativeClassFactory();
        //
        // _decrypterProvider.UseKeyFile(_conversionConfig.KeysPath);
        // var packageUnpacker = rlServices.GetPackageUnpacker(_decrypterProvider);
        // var cacheOptions = new PackageCacheOptions(rlServices.UnrealPackageSerializer, nativeFactory)
        // {
        //     SearchPaths =
        //     {
        //         _conversionConfig.ImportPackagesDirectory
        //     },
        //     GraphLinkPackages = true,
        //     PackageUnpacker = packageUnpacker,
        //     NativeClassFactory = nativeFactory,
        //     ObjectSerializerFactory = rlServices.ObjectSerializerFactory,
        //     PackageBlacklist =
        //     {
        //         "EngineMaterials", "EngineResources"
        //     }
        // };
        // var packageCache = new PackageCache(cacheOptions);
        //
        // var loader = new PackageLoader(rlServices.UnrealPackageSerializer, packageCache, packageUnpacker, nativeFactory,
        //     rlServices.ObjectSerializerFactory);
        // var exporterFactory = udkServices.PackageExporterFactory;

        // var packageConverter = new PackageConverter(_conversionConfig, exporterFactory, loader,
        //     _conversionConfig.Compress ? GetDefaultPackageCompressor() : null, null);
        // var packageConverter = _packageConverterFactory.Create(_conversionConfig);
        // if (packageConverter is null)
        // {
        //     _logger.LogError("Failed to create package converter");
        //     return;
        // }


        ConcurrentQueue<FileReference> processQueue = new(FileReferences);

        var sw = Stopwatch.StartNew();
        
        var processTasks = Enumerable.Range(0, 2).Select(i =>
        {
            return Task.Run(() =>
            {
                var converter = _packageConverterFactory.Create(_conversionConfig);
                while (processQueue.TryDequeue(out var file))
                {
                    converter!.ProcessFile(file);
                }
            });
        });
        await Task.WhenAll(processTasks);

        // var filePartitions = Split(FileReferences, 8).ToList();
        // var counts = filePartitions.Sum(x => x.Count());
        // var conversionTasks = new List<Task>();

        // foreach (var files in filePartitions)
        // {
        //     var task = Task.Run(() =>
        //     {
        //         var converter = _packageConverterFactory.Create(_conversionConfig);
        //         foreach (var file in files)
        //         {
        //             converter!.ProcessFile(file);
        //         }
        //     });
        //     conversionTasks.Add(task);
        // }

        sw.Stop();
        _logger.LogInformation("Processed {FileCount} in {Duration} s", FileReferences.Count, sw.Elapsed.TotalSeconds);



        // _packageConverterFactory.PreloadCommonPackages(_conversionConfig);

        // Parallel.ForEach(filePartitions, new ParallelOptions{MaxDegreeOfParallelism = 32}, (source, state) =>
        // {
        //     var converter = _packageConverterFactory.Create(_conversionConfig);
        //     foreach (var file in source)
        //     {
        //         converter!.ProcessFile(file);
        //     }
        // });


        //     Parallel.ForEach<FileReference, PackageConverter?>(
        //         FileReferences,
        //          new ParallelOptions{MaxDegreeOfParallelism = 16},
        //         () =>
        //         {
        //             _logger.LogInformation("Creating new PackageConverter on thread {ThreadID}", Environment.CurrentManagedThreadId);
        //             return _packageConverterFactory.Create(_conversionConfig);
        //         },
        //         (reference, state, packageConverter) =>
        //         {
        //             packageConverter?.ProcessFile(reference);
        //             return packageConverter;
        //         }, _ => { });
    }

    public static IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> list, int parts)
    {
        int i = 0;
        var splits = list.GroupBy(item => i++ % parts).Select(part => part.AsEnumerable());
        return splits;
    }
}