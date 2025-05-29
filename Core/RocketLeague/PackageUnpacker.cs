using RlUpk.Core.RocketLeague.Decryption;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types;
using RlUpk.Core.Types.FileSummeryInner;

namespace RlUpk.Core.RocketLeague;

/// <summary>
///     Unpack compressed and encrypted RL packages
/// </summary>
public class PackageUnpacker : IPackageUnpacker
{
    private readonly IDecrypterProvider _decryptionProvider;
    private readonly IStreamSerializer<FileSummary> _fileSummarySerializer;

    /// <summary>
    ///     Initialize with decryption and file summary serializers
    /// </summary>
    /// <param name="fileSummarySerializer"></param>
    /// <param name="decryptionProvider"></param>
    public PackageUnpacker(IStreamSerializer<FileSummary> fileSummarySerializer, IDecrypterProvider decryptionProvider)
    {
        _fileSummarySerializer = fileSummarySerializer;
        _decryptionProvider = decryptionProvider;
    }

    /// <summary>
    ///     Unpack the package to the stream
    /// </summary>
    /// <param name="packageStream"></param>
    /// <param name="outputStream"></param>
    /// <returns></returns>
    public UnpackResult Unpack(Stream packageStream, Stream outputStream)
    {
        var packageUnpacker = new RLPackageUnpacker(packageStream, _decryptionProvider, _fileSummarySerializer);
        return packageUnpacker.Unpack(outputStream);
    }

    /// <summary>
    ///     Checks the file summary for the compression flag
    /// </summary>
    /// <param name="packageStream"></param>
    /// <returns></returns>
    public bool IsPackagePacked(Stream packageStream)
    {
        using var streamTempPosition = packageStream.TemporarySeek();
        var fileSummary = _fileSummarySerializer.Deserialize(packageStream);
        return fileSummary.CompressionFlags != ECompressionFlags.CompressNone;
    }
}