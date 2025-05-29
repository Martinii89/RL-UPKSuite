namespace RlUpk.Core.Flags;

/// <summary>
///     Flags describing an property instance.
///     Note:
///     This is valid for UE3 as well unless otherwise noted.
///     @Redefined( Version, Clone )
///     The flag is redefined in (Version) as (Clone)
///     @Removed( Version )
///     The flag is removed in (Version)
/// </summary>
[Flags]
public enum PropertyFlagsLO : ulong // actually uint but were using ulong for UE2 and UE3 Compatibly
{
    /// <summary>
    ///     The parameter is optional.
    /// </summary>
    OptionalParm = 0x00000010U,

    Parm = 0x00000080U, // Property is a part of the function parameters

    OutParm = 0x00000100U, // Reference(UE3) param

    SkipParm = 0x00000200U, // ???

    /// <summary>
    ///     The property is a return type
    /// </summary>
    ReturnParm = 0x00000400U,

    CoerceParm = 0x00000800U, // auto-cast

    Editable = 0x00000001U, // Can be set by UnrealEd users

    Const = 0x00000002U, // ReadOnly

    /// <summary>
    ///     UE2
    /// </summary>
    Input = 0x00000004U, // Can be set with binds
    ExportObject = 0x00000008U, // Export suboject properties to clipboard
    Net = 0x00000020U, // Replicated

    EditConstArray = 0x00000040U, // Dynamic Array size cannot be changed by UnrealEd users
    EditFixedSize = 0x00000040U,

    Native = 0x00001000U, // C++
    Transient = 0x00002000U, // Don't save
    Config = 0x00004000U, // Saved within .ini
    Localized = 0x00008000U, // Language ...
    Travel = 0x00010000U, // Keep value after travel
    EditConst = 0x00020000U, // ReadOnly in UnrealEd

    GlobalConfig = 0x00040000U,

    /// <summary>
    ///     The property is a component.
    ///     => UE3
    /// </summary>
    Component = 0x00080000U, // NetAlways in 61 <=
    OnDemand = 0x00100000U, // @Redefined(UE3, Init) Load on demand
    Init = 0x00100000U, //

    New = 0x00200000U, // Inner object. @Removed(UE3)
    DuplicateTransient = 0x00200000U,

    NeedCtorLink = 0x00400000U,
    NoExport = 0x00800000U, // Don't export properties to clipboard

    EditorData = 0x02000000U, // @Redefined(UE3, NoClear)
    NoClear = 0x02000000U, // Don't permit reference clearing.

    EditInline = 0x04000000U,
    EdFindable = 0x08000000U,
    EditInlineUse = 0x10000000U,
    Deprecated = 0x20000000U,

    EditInlineNotify = 0x40000000U, // Always set on Automated tagged properties (name is assumed!)
    DataBinding = 0x40000000U,

    SerializeText = 0x80000000U,

    #region UT2004 Flags

    Cache = 0x01000000U, // @Removed(UE3) Generate cache file: .ucl
    NoImport = 0x01000000U,
    Automated = 0x80000000U, // @Removed(UE3)

    #endregion

    #region Combinations

    EditInlineAll = EditInline | EditInlineUse,
    Instanced = ExportObject | EditInline,

    #endregion
}