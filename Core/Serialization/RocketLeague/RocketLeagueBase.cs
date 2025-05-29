namespace RlUpk.Core.Serialization.RocketLeague;

/// <summary>
///     Base for rocket league specific serializers. It's only purpose is to place the correct FileVersion attribute
/// </summary>
[FileVersion(FileVersion)]
public class RocketLeagueBase
{
    /// <summary>
    ///     A constant for the FileVersion so we can avoid magic strings.
    /// </summary>
    public const string FileVersion = "RocketLeague";
}