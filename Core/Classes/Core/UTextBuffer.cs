using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

/// <summary>
///     A text buffer object?
/// </summary>
[NativeOnlyClass("Core", "TextBuffer", typeof(UObject))]
public class UTextBuffer : UObject
{
    /// <inheritdoc />
    public UTextBuffer(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public uint Pos { get; set; }
    public uint Top { get; set; }
    public string ScriptText { get; set; } = string.Empty;
}