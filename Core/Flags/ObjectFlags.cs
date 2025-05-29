namespace RlUpk.Core.Flags;

public enum ObjectFlagsLO : ulong // 32bit aligned, see ObjectFlags64
{
    Transactional = 0x00000001U,
    Public = 0x00000004U,

    Private = 0x00000080U,
    Automated = 0x00000100U,

    Transient = 0x00004000U,

    LoadForClient = 0x00010000U,
    LoadForServer = 0x00020000U,
    LoadForEdit = 0x00040000U,
    Standalone = 0x00080000U,
    NotForClient = 0x00100000U,
    NotForServer = 0x00200000U,
    NotForEdit = 0x00400000U,

    HasStack = 0x02000000U,
    Native = 0x04000000U,

    Marked = 0x08000000U
    //SWAT4_Unnamed         = 0x08000000U,
}

public enum ObjectFlagsHO : ulong // 32bit aligned, see ObjectFlags
{
    Obsolete = 0x00000020U,
    Final = 0x00000080U,
    PerObjectLocalized = 0x00000100U,
    PropertiesObject = 0x00000200U,
    ArchetypeObject = 0x00000400U,
    RemappedName = 0x00000800U
}