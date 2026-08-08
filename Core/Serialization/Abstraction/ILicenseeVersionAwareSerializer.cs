namespace RlUpk.Core.Serialization.Abstraction;

/// <summary>
///     A serializer whose format depends on the package licensee version.
/// </summary>
public interface ILicenseeVersionAwareSerializer
{
    /// <summary>
    ///     The licensee version of the package being serialized.
    /// </summary>
    ushort LicenseeVersion { get; set; }
}
