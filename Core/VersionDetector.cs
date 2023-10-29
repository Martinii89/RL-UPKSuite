using Core.Serialization.RocketLeague;

namespace Core;

/// <summary>
///     Helper class to detect the build of a package
/// </summary>
public class VersionDetector
{
    /// <summary>
    ///     Reads the first few bytes of the stream and detects the version. Rewinds the stream to the original position after
    ///     reading
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static string GetBuildOfStream(Stream stream)
    {
        using var tmp = stream.TemporarySeek();
        var tag = stream.ReadInt32();
        var fileVersion = stream.ReadInt16();
        var licenseeVersion = stream.ReadInt16();

        // UDK
        return GetBuild(fileVersion, licenseeVersion);
    }

    private static string GetBuild(short fileVersion, short licenseeVersion)
    {
        if (fileVersion is 868 or 867 && licenseeVersion == 0)
        {
            return "";
        }

        if (fileVersion == 868 && licenseeVersion is >= 18 and <= 32)
        {
            return RocketLeagueBase.FileVersion;
        }

        throw new IndexOutOfRangeException("Unknown build");
    }

    /// <summary>
    ///     Reads the first few bytes of a file and detects the version
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetBuildOfFile(string filePath)
    {
        return GetBuildOfStream(File.OpenRead(filePath));
    }
}