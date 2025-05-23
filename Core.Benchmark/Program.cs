using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Core;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization;
using Core.Serialization.RocketLeague;
using Core.Test.TestUtilities;
using Core.Tests;
using Core.Types;
using Core.Utility;

BenchmarkRunner.Run<PackageLoadingBenchmark>();

// [MemoryDiagnoser]
public class PackageLoadingBenchmark
{
    private PackageLoader _loader;
    private IStreamSerializer<UnrealPackage> packageSerializer;
    private PackageUnpacker unpacker;
    private NativeClassFactory nativeFactory;
    private PackageCacheOptions options;

    [GlobalSetup]
    public void Setup()
    {
        var fileSummarySerializer = SerializerHelper.GetSerializerFor<FileSummary>(typeof(FileSummary), RocketLeagueBase.FileVersion);
        unpacker = new PackageUnpacker(fileSummarySerializer, new DecryptionProvider("keys.txt"));
        packageSerializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage), RocketLeagueBase.FileVersion);
        nativeFactory = new NativeClassFactory();
        options = new PackageCacheOptions(packageSerializer, nativeFactory)
        {
            SearchPaths = { TestConstants.CookedPCConsolePath }, GraphLinkPackages = false, PackageUnpacker = unpacker
        };
    }

    [Benchmark]
    public UnrealPackage LoadTAGamePackage()
    {
        var packageCache = new PackageCache(options);
        _loader = new PackageLoader(packageSerializer, packageCache, unpacker, nativeFactory);
        _loader.LoadPackage(TestConstants.TAGamePath, "TAGame");
        return _loader.GetPackage("TAGame");
    }
}