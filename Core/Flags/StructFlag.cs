namespace Core.Flags;

[Flags]
public enum StructFlag
{
    Native = 0x00000001,
    Export = 0x00000002,
    HasComponents = 0x00000004,
    Transient = 0x00000008,
    Atomic = 0x00000010,
    Immutable = 0x00000020,
    StrictConfig = 0x00000040,
    ImmutableWhenCooked = 0x00000080,
    AtomicWhenCooked = 0x00000100,
    Inherit = HasComponents | Atomic | AtomicWhenCooked | StrictConfig
}