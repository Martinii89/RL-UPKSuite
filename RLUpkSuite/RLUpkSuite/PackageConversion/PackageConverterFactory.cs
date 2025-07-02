using Microsoft.Extensions.Logging;

using RlUpk.Core;
using RlUpk.Core.Classes;
using RlUpk.Core.Classes.Compression;
using RlUpk.Core.RocketLeague.Decryption;
using RlUpk.Core.Serialization.Default;
using RlUpk.Core.Utility;
using RlUpk.RLUpkSuite.Config;

namespace RlUpk.RLUpkSuite.PackageConversion;

public class PackageConverterFactory
{
    private readonly Func<IDecrypterProvider> _decrypterFactory;

    private readonly ILogger<PackageConverter> _logger;

    private readonly PackageCompressor _packageCompressor;

    private readonly RlSerializerCollection _rlSerializerCollection;

    private readonly UdkSerializerCollection _udkSerializerCollection;


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
        FileSummarySerializer headerSerializer = FileSummarySerializer.GetDefaultSerializer();
        ExportTableItemSerializer exportTableIteSerializer =
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

        IDecrypterProvider decrypter = _decrypterFactory();
        decrypter.UseKeyFile(config.KeysPath);
        NativeClassFactory nativeClassFactory = new NativeClassFactory();
        PackageCache packageCache = new PackageCache(CreateCacheOptions(config, decrypter, nativeClassFactory));
        

        PackageLoader loader = new PackageLoader(_rlSerializerCollection.UnrealPackageSerializer, packageCache,
            _rlSerializerCollection.GetPackageUnpacker(decrypter), nativeClassFactory,
            _rlSerializerCollection.ObjectSerializerFactory);
        PackageExporterFactory exporterFactory = _udkSerializerCollection.PackageExporterFactory;

        PackageConverter packageConverter = new PackageConverter(config, exporterFactory, loader,
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