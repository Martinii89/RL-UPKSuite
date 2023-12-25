using Core.Classes.Compression;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Serialization.Default;
using Core.Serialization.RocketLeague;
using Core.Types;

using Microsoft.Extensions.DependencyInjection;

namespace RLUpkSuite.PackageConversion;

public class RlSerializerCollection
{

    private readonly ServiceProvider _services;

    public RlSerializerCollection()
    {
        //It's a bit of a hack to init a temp service collection here.
        // These needs to be resolved from a clean service collection
        // populated with the right serializers based on the UseSerializers call
        var serviceCollection = new ServiceCollection();
        serviceCollection.UseSerializers(typeof(UnrealPackage), new SerializerOptions(RocketLeagueBase.FileVersion));
        serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
        _services = serviceCollection.BuildServiceProvider();
        
        FileSummarySerializer = _services.GetRequiredService<IStreamSerializer<FileSummary>>();
        UnrealPackageSerializer = _services.GetRequiredService<IStreamSerializer<UnrealPackage>>();
        ObjectSerializerFactory = _services.GetRequiredService<IObjectSerializerFactory>();
    }

    public PackageUnpacker GetPackageUnpacker(IDecrypterProvider decryptionProvider)
    {
        return new PackageUnpacker(FileSummarySerializer, decryptionProvider);
    }


    public IStreamSerializer<FileSummary> FileSummarySerializer { get; private set; }
    public IStreamSerializer<UnrealPackage> UnrealPackageSerializer { get; private set; }
    public IObjectSerializerFactory ObjectSerializerFactory { get; private set; }
}

public class DefaultPackageCompressor
{
    DefaultPackageCompressor()
    {
        var headerSerializer = FileSummarySerializer.GetDefaultSerializer();
        var exportTableIteSerializer =
            new Core.Serialization.Default.ExportTableItemSerializer(new FNameSerializer(), new ObjectIndexSerializer(),
                new FGuidSerializer());
        PackageCompressor = new PackageCompressor(headerSerializer, exportTableIteSerializer,
            new FCompressedChunkinfoSerializer());
    }

    public PackageCompressor PackageCompressor { get; }

}