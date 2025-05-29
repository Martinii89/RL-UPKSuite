using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization;

/// <summary>
///     A <see cref="FileVersionAttribute" /> is a tag used to mark <see cref="IStreamSerializer{T}" /> implementations.
///     It tells which version of a file format the serializers is compatible with
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class FileVersionAttribute : Attribute
{
    /// <summary>
    ///     The Compatible version
    /// </summary>
    public readonly string FileVersion;

    /// <summary>
    ///     Constructor for the Attribute taking in the file version as a string
    /// </summary>
    /// <param name="fileVersion"></param>
    public FileVersionAttribute(string fileVersion)
    {
        FileVersion = fileVersion;
    }
}