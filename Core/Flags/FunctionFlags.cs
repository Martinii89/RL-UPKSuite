namespace Core.Flags;

/// <summary>
///     Flags describing an function instance.
/// </summary>
[Flags]
public enum FunctionFlags : ulong // actually uint but were using ulong for UE2 and UE3 Compatably
{
    Final = 0x00000001U,
    Defined = 0x00000002U,
    Iterator = 0x00000004U,
    Latent = 0x00000008U,
    PreOperator = 0x00000010U,
    Singular = 0x00000020U,
    Net = 0x00000040U,
    NetReliable = 0x00000080U,
    Simulated = 0x00000100U,
    Exec = 0x00000200U,
    Native = 0x00000400U,
    Event = 0x00000800U,
    Operator = 0x00001000U,
    Static = 0x00002000U,
    NoExport = 0x00004000U, // Can also be an identifier for functions with Optional parameters.
    OptionalParameters = 0x00004000U,
    Const = 0x00008000U,
    Invariant = 0x00010000U,
    Public = 0x00020000U,
    Private = 0x00040000U,
    Protected = 0x00080000U,
    Delegate = 0x00100000U,
    NetServer = 0x00200000U,

    NetClient = 0x01000000U,
    DLLImport = 0x02000000U, // Also available in UE2(unknown meaning there)
    K2Call = 0x04000000U,
    K2Override = 0x08000000U, // K2Call?
    K2Pure = 0x10000000U
}