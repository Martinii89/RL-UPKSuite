namespace RlUpk.Core;

/// <summary>
///     Interface for classes able to unpack packages
/// </summary>
public interface IPackageUnpacker
{
    /// <summary>
    ///     Unpack the package and fill the output stream with the unpacked package data.
    /// </summary>
    /// <returns></returns>
    UnpackResult Unpack(Stream packageStream, Stream outputStream);

    /// <summary>
    ///     Checks if the package requires unpacking
    /// </summary>
    /// <param name="packageStream"></param>
    /// <returns></returns>
    bool IsPackagePacked(Stream packageStream);
}

/// <summary>
///     Indicates the result of a package unpacking.
/// </summary>
[Flags]
public enum UnpackResult
{
    /// <summary>
    ///     No unpacking has been done
    /// </summary>
    None = 0,

    /// <summary>
    ///     Header has been deserialized
    /// </summary>
    Header = 1,

    /// <summary>
    ///     Encrypted data has been decrypted
    /// </summary>
    Decrypted = 2,

    /// <summary>
    ///     Compressed data has been uncompressed
    /// </summary>
    Inflated = 4,

    /// <summary>
    ///     Unpacking was successful
    /// </summary>
    Success = Header | Decrypted | Inflated
}