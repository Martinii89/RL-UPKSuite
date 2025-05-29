using System.Diagnostics;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Core;
using Core.Benchmark;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization;
using Core.Serialization.RocketLeague;
using Core.Test.TestUtilities;
using Core.Tests;
using Core.Types;
using Core.Utility;

// BenchmarkRunner.Run<UnpackBenchmark>();
BenchmarkRunner.Run<IOBenchmark>();
// BenchmarkRunner.Run<PackageLoadingBenchmark>();

// for (int i = 0; i < 1; i++)
// {
//     var test = new PackageLoadingBenchmark();
//     test.Setup();
//     var pckg = test.LoadTAGamePackage();
//     Console.WriteLine(pckg.PackageName);
// }


// string testPackage = "TestData/RocketPass_Premium_T_SF.upk";
// var test = new UnpackBenchmark { InputFilePath = testPackage };
// test.Setup();
// var test2 = new UnpackBenchmark { InputFilePath = testPackage };
// test2.Setup();
// var s = 0L;
// var sw = Stopwatch.StartNew();
// while (sw.Elapsed.Seconds < 30)
// {
//     var pckg = test.UnpackV2();
//     var pckg2= test2.Unpack();
//     s += pckg.Length + pckg2.Length;
// }
// Console.WriteLine(s);

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