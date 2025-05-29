namespace RlUpk.TestUtils.TestUtilities;

public class PackageStreamFixture
{
    private readonly byte[] _coreBytes;
    private readonly byte[] _customGameBytes;
    private readonly byte[] _engineBytes;

    public PackageStreamFixture()
    {
        _coreBytes = File.ReadAllBytes(@"TestData/UDK/Core.u");
        _engineBytes = File.ReadAllBytes("TestData/UDK/Engine.u");
        _customGameBytes = File.ReadAllBytes(@"TestData/UDK/CustomGame.u");
    }

    public Stream CoreStream => new MemoryStream(_coreBytes, false);
    public Stream EngineStream => new MemoryStream(_engineBytes, false);
    public Stream CustomGameStream => new MemoryStream(_customGameBytes, false);
}