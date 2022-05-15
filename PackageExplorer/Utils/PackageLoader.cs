using System.Collections.Generic;
using System.IO;
using Core;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization;
using Core.Serialization.RocketLeague;
using Core.Types;
using Core.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace PackageExplorer.Utils;

public class PackageLoader
{
    public static UnrealPackage? LoadPackage(string packageFilePath, string packageName, List<string>? searchPaths, List<string>? packageExtensions)
    {
        string version;
        {
            using var packageStream = File.OpenRead(packageFilePath);
            version = VersionDetector.GetBuildOfStream(packageStream);
        }


        var serviceCollection = new ServiceCollection();
        serviceCollection.UseSerializers(typeof(UnrealPackage), new SerializerOptions(version));
        var services = serviceCollection.BuildServiceProvider();

        var serializer = services.GetRequiredService<IStreamSerializerFor<UnrealPackage>>();
        var fileSummaryserializer = services.GetRequiredService<IStreamSerializerFor<FileSummary>>();

        IPackageUnpacker unpacker = version == RocketLeagueBase.FileVersion
            ? new PackageUnpacker(fileSummaryserializer, new DecryptionProvider("keys.txt"))
            : new NeverUnpackUnpacker();
        searchPaths ??= new List<string> { Path.GetDirectoryName(packageFilePath) ?? string.Empty };
        packageExtensions ??= new List<string> { "upk", "u" };
        var options = new PackageCacheOptions(serializer)
            { SearchPaths = searchPaths, Extensions = packageExtensions, GraphLinkPackages = false, PackageUnpacker = unpacker };
        var packageCache = new PackageCache(options);
        var loader = new Core.PackageLoader(serializer, packageCache, unpacker);

        loader.LoadPackage(packageFilePath, packageName);
        return loader.GetPackage(packageName);
    }
}