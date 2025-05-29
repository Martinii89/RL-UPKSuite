using Microsoft.Extensions.DependencyInjection;

using RlUpk.Core.Classes.Compression;
using RlUpk.Core.RocketLeague;
using RlUpk.Core.RocketLeague.Decryption;
using RlUpk.Core.Serialization;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Default;
using RlUpk.Core.Serialization.Extensions;
using RlUpk.Core.Serialization.RocketLeague;
using RlUpk.Core.Types;

using ExportTableItemSerializer = RlUpk.Core.Serialization.Default.ExportTableItemSerializer;

namespace RlUpk.RLUpkSuite.PackageConversion;

public class RlSerializerCollection
{
    private readonly ServiceProvider _services;

    public RlSerializerCollection()
    {
        //It's a bit of a hack to init a temp service collection here.
        // These needs to be resolved from a clean service collection
        // populated with the right serializers based on the UseSerializers call
        ServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddSerializers(typeof(UnrealPackage), new SerializerOptions(RocketLeagueBase.FileVersion));
        serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
        _services = serviceCollection.BuildServiceProvider();

        FileSummarySerializer = _services.GetRequiredService<IStreamSerializer<FileSummary>>();
        UnrealPackageSerializer = _services.GetRequiredService<IStreamSerializer<UnrealPackage>>();
        ObjectSerializerFactory = _services.GetRequiredService<IObjectSerializerFactory>();
    }


    public IStreamSerializer<FileSummary> FileSummarySerializer { get; }

    public IStreamSerializer<UnrealPackage> UnrealPackageSerializer { get; private set; }

    public IObjectSerializerFactory ObjectSerializerFactory { get; private set; }

    public PackageUnpacker GetPackageUnpacker(IDecrypterProvider decryptionProvider)
    {
        return new PackageUnpacker(FileSummarySerializer, decryptionProvider);
    }
}

public class DefaultPackageCompressor
{
    private DefaultPackageCompressor()
    {
        FileSummarySerializer headerSerializer = FileSummarySerializer.GetDefaultSerializer();
        ExportTableItemSerializer exportTableIteSerializer =
            new ExportTableItemSerializer(new FNameSerializer(), new ObjectIndexSerializer(),
                new FGuidSerializer());
        PackageCompressor = new PackageCompressor(headerSerializer, exportTableIteSerializer,
            new FCompressedChunkinfoSerializer());
    }

    public PackageCompressor PackageCompressor { get; }
}