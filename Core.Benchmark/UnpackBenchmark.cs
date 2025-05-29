using BenchmarkDotNet.Attributes;

using RlUpk.Core.RocketLeague;
using RlUpk.Core.RocketLeague.Decryption;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.RocketLeague;
using RlUpk.Core.Types;
using RlUpk.TestUtils.TestUtilities;

namespace RlUpk.Core.Benchmark;


// [ShortRunJob]
[MemoryDiagnoser]
public class UnpackBenchmark
{
    [Params(
        "TestData/TAGame.upk",
        "TestData/RocketPass_Premium_T_SF.upk",
        "TestData/IpDrv.upk"
    )]
    public string InputFilePath;

    private Stream input;
    private DecryptionProvider _decryptionProvider;
    private IStreamSerializer<FileSummary> _serializer;

    [GlobalSetup]
    public void Setup()
    {
        input = File.OpenRead(InputFilePath);
        _decryptionProvider = new DecryptionProvider("keys.txt");
        _serializer = SerializerHelper.GetSerializerFor<FileSummary>(typeof(FileSummary), RocketLeagueBase.FileVersion);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        input.Dispose();
    }

    [Benchmark(Baseline = true)]
    public Stream Unpack()
    {
        input.Seek(0, SeekOrigin.Begin);
        var packedFile = new RLPackageUnpacker(input, _decryptionProvider, _serializer);
        var output = new MemoryStream();
        packedFile.Unpack(output);
        return output;
    }

    [Benchmark]
    public Stream UnpackV2()
    {
        input.Seek(0, SeekOrigin.Begin);
        var packedFile = new RLPackageUnpackerV2(_decryptionProvider, _serializer);
        var output = new MemoryStream();
        packedFile.Unpack(input, output);
        return output;
    }
}
