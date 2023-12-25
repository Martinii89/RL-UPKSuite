using Core;
using Core.Classes.Compression;
using Core.RocketLeague.Decryption;
using Core.Serialization.Default;
using Core.Utility;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using RLUpkSuite.Config;

namespace RLUpkSuite.PackageConversion;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPackageConversion(this IServiceCollection services)
    {
        services.TryAddSingleton<UdkSerializerCollection>();
        services.TryAddSingleton<RlSerializerCollection>();
        services.TryAddTransient<IDecrypterProvider, DecryptionProvider>();
        
        // services.AddSingleton<Func<NativeClassFactory>>(() => new NativeClassFactory());
        services.AddSingleton<Func<IDecrypterProvider>>((provider => provider.GetRequiredService<IDecrypterProvider>));
        services.TryAddSingleton<PackageConverterFactory>();

        return services;
    }
}

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


    public PackageConverter? Create(ConversionConfig config)
    {
        if (config.KeysPath is null || config.ImportPackagesDirectory is null)
        {
            return null;
        }

        var rlServices = new RlSerializerCollection(); // _rlSerializerCollection;
        var udkServices = new UdkSerializerCollection(); // _udkSerializerCollection;
        var nativeFactory = new NativeClassFactory();

        var decrypter = new DecryptionProvider(); // _decrypterFactory();
        decrypter.UseKeyFile(config.KeysPath);
        var packageUnpacker = rlServices.GetPackageUnpacker(decrypter);
        var cacheOptions = new PackageCacheOptions(rlServices.UnrealPackageSerializer, nativeFactory)
        {
            SearchPaths =
            {
                config.ImportPackagesDirectory
            },
            GraphLinkPackages = true,
            PackageUnpacker = packageUnpacker,
            NativeClassFactory = nativeFactory,
            ObjectSerializerFactory = rlServices.ObjectSerializerFactory,
            PackageBlacklist =
            {
                "EngineMaterials", "EngineResources"
            }
        };
        var packageCache = new PackageCache(cacheOptions);

        var loader = new PackageLoader(rlServices.UnrealPackageSerializer, packageCache, packageUnpacker, nativeFactory,
            rlServices.ObjectSerializerFactory);
        var exporterFactory = udkServices.PackageExporterFactory;

        var packageConverter = new PackageConverter(config, exporterFactory, loader,
            config.Compress ? GetDefaultPackageCompressor() : null, _logger);

        return packageConverter;
    }
}