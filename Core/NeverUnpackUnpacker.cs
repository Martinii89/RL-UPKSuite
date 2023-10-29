namespace Core;

/// <summary>
///     Default implementation for when you know the package never requires unpacking
/// </summary>
public class NeverUnpackUnpacker : IPackageUnpacker
{
    public UnpackResult Unpack(Stream packageStream, Stream outputStream)
    {
        throw new NotImplementedException();
    }

    public bool IsPackagePacked(Stream packageStream)
    {
        return false;
    }
}