using System.IO;

using Core;
using Core.Classes.Compression;
using Core.RocketLeague.Decryption;
using Core.Serialization.Default;
using Core.Types.PackageTables;
using Core.Utility;

using Microsoft.Extensions.Logging;

using RLUpkSuite.Config;

namespace RLUpkSuite.PackageConversion;

public class PackageConverterFactory
{
    private readonly ILogger<PackageConverter> _logger;

    private readonly PackageCompressor _packageCompressor;

    private readonly UdkSerializerCollection _udkSerializerCollection;

    private readonly RlSerializerCollection _rlSerializerCollection;

    private readonly Func<IDecrypterProvider> _decrypterFactory;


    public PackageConverterFactory(ILogger<PackageConverter> logger, UdkSerializerCollection udkSerializerCollection,
        RlSerializerCollection rlSerializerCollection, Func<IDecrypterProvider> decrypterFactory)
    {
        _logger = logger;
        _udkSerializerCollection = udkSerializerCollection;
        _rlSerializerCollection = rlSerializerCollection;
        _decrypterFactory = decrypterFactory;
        _packageCompressor = GetDefaultPackageCompressor();
    }

    private PackageCompressor GetDefaultPackageCompressor()
    {
        var headerSerializer = FileSummarySerializer.GetDefaultSerializer();
        var exportTableIteSerializer =
            new ExportTableItemSerializer(new FNameSerializer(), new ObjectIndexSerializer(),
                new FGuidSerializer());
        return new PackageCompressor(headerSerializer, exportTableIteSerializer, new FCompressedChunkinfoSerializer());
    }

    // public void PreloadCommonPackages(ConversionConfig config)
    // {
    //     if (config.KeysPath is null || config.ImportPackagesDirectory is null)
    //     {
    //         return;
    //     }
    //     
    //     var decrypter = _decrypterFactory();
    //     decrypter.UseKeyFile(config.KeysPath);
    //     this.PreloadPackageCache = new PackageCache(CreateCacheOptions(config, decrypter));
    //     var rlServices = new RlSerializerCollection(); // _rlSerializerCollection;
    //     var packageUnpacker = rlServices.GetPackageUnpacker(decrypter);
    //     var nativeFactory = new NativeClassFactory();
    //     var loader = new PackageLoader(rlServices.UnrealPackageSerializer, PreloadPackageCache, packageUnpacker, nativeFactory,
    //         rlServices.ObjectSerializerFactory);
    //
    //     loader.LoadPackage( Path.Combine(config.ImportPackagesDirectory, "tagame.upk"), "tagame");
    //     // foreach (string cachedPackageName in PreloadPackageCache.GetCachedPackageNames())
    //     // {
    //     //     var package = PreloadPackageCache.GetCachedPackage(cachedPackageName);
    //     //     foreach (ImportTableItem import in package.ImportTable)
    //     //     {
    //     //         import.ImportedObject?.Deserialize();
    //     //     }
    //     //
    //     //     foreach (ExportTableItem exportTableItem in package.ExportTable)
    //     //     {
    //     //         exportTableItem.Object?.Deserialize();
    //     //     }
    //     // }
    // }

    // private PackageCache PreloadPackageCache { get; set; }


    public PackageConverter? Create(ConversionConfig config)
    {
        if (config.KeysPath is null || config.ImportPackagesDirectory is null)
        {
            return null;
        }

        var decrypter = _decrypterFactory();
        decrypter.UseKeyFile(config.KeysPath);
        var nativeClassFactory = new NativeClassFactory();
        var packageCache = new PackageCache(CreateCacheOptions(config, decrypter, nativeClassFactory));

        // foreach (string cachedPackageName in PreloadPackageCache.GetCachedPackageNames())
        // {
        //     packageCache.AddPackage(PreloadPackageCache.GetCachedPackage(cachedPackageName));
        // }

        var loader = new PackageLoader(_rlSerializerCollection.UnrealPackageSerializer, packageCache,
            _rlSerializerCollection.GetPackageUnpacker(decrypter), nativeClassFactory,
            _rlSerializerCollection.ObjectSerializerFactory);
        var exporterFactory = _udkSerializerCollection.PackageExporterFactory;

        var packageConverter = new PackageConverter(config, exporterFactory, loader,
            config.Compress ? GetDefaultPackageCompressor() : null, _logger);

        return packageConverter;
    }

    private PackageCacheOptions CreateCacheOptions(ConversionConfig config, IDecrypterProvider decrypter,
        NativeClassFactory nativeClassFactory)
    {
        return new PackageCacheOptions(_rlSerializerCollection.UnrealPackageSerializer, nativeClassFactory)
        {
            SearchPaths = [config.ImportPackagesDirectory],
            PackageUnpacker = _rlSerializerCollection.GetPackageUnpacker(decrypter),
            ObjectSerializerFactory = _rlSerializerCollection.ObjectSerializerFactory,
            PackageBlacklist = ["EngineMaterials", "EngineResources"]
        };
    }
}