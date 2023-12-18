using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using CommunityToolkit.Mvvm.Input;

using Core;
using Core.Classes.Compression;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Serialization.Default;
using Core.Serialization.RocketLeague;
using Core.Types;
using Core.Utility;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.DependencyInjection;

using RLUpkSuite.Config;
using RLUpkSuite.PackageConversion;
using RLUpkSuite.Pages;

using ExportTableItemSerializer = Core.Serialization.RocketLeague.ExportTableItemSerializer;

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
        await Task.Run(CovertFilesImpl);
        if (_conversionConfig.OpenOutputOnFinish && OutputDirectory != null)
        {
            Process.Start("explorer.exe", OutputDirectory);
        }
    }

    private void CovertFilesImpl()
    {
        if (_conversionConfig.KeysPath is null
            || _conversionConfig.ImportPackagesDirectory is null)
        {
            return;
        }

        var rlServices = GetRLSerializerCollection();
        var udkServices = GetUdkSerializerCollection();

        var rlFileSummarySerializer = rlServices.GetRequiredService<IStreamSerializer<FileSummary>>();
        var rlPackageSerializer = rlServices.GetRequiredService<IStreamSerializer<UnrealPackage>>();
        var rLobjectSerializerFactory = rlServices.GetRequiredService<IObjectSerializerFactory>();

        var unpacker = new PackageUnpacker(rlFileSummarySerializer, new DecryptionProvider(_conversionConfig.KeysPath));
        var nativeFactory = new NativeClassFactory();


        var cacheOptions = new PackageCacheOptions(rlPackageSerializer, nativeFactory)
        {
            SearchPaths =
            {
                _conversionConfig.ImportPackagesDirectory
            },
            GraphLinkPackages = true,
            PackageUnpacker = unpacker,
            NativeClassFactory = nativeFactory,
            ObjectSerializerFactory = rLobjectSerializerFactory,
            PackageBlacklist =
            {
                "EngineMaterials", "EngineResources"
            }
        };
        var packageCache = new PackageCache(cacheOptions);

        var loader = new PackageLoader(rlPackageSerializer, packageCache, unpacker, nativeFactory,
            rLobjectSerializerFactory);
        var exporterFactory = udkServices.GetRequiredService<PackageExporterFactory>();

        var packageConverter = new PackageConverter(_conversionConfig, exporterFactory, loader,
            _conversionConfig.Compress ? GetDefaultPackageCompressor() : null);
        packageConverter.SetFiles(FileReferences);
        packageConverter.Start();
    }

    private IServiceProvider GetUdkSerializerCollection()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.UseSerializers(typeof(UnrealPackage), new SerializerOptions());
        serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
        serviceCollection.AddSingleton<PackageExporterFactory>();
        var services = serviceCollection.BuildServiceProvider();
        return services;
    }

    private IServiceProvider GetRLSerializerCollection()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.UseSerializers(typeof(UnrealPackage), new SerializerOptions(RocketLeagueBase.FileVersion));
        serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
        var services = serviceCollection.BuildServiceProvider();
        return services;
    }
    
    PackageCompressor GetDefaultPackageCompressor()
    {
        var headerSerializer = FileSummarySerializer.GetDefaultSerializer();
        var exportTableIteSerializer =
            new Core.Serialization.Default.ExportTableItemSerializer(new FNameSerializer(), new ObjectIndexSerializer(), new FGuidSerializer());
        return new PackageCompressor(headerSerializer, exportTableIteSerializer, new FCompressedChunkinfoSerializer());
    }
}