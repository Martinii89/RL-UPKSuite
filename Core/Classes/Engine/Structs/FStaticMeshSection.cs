using Core.Types.PackageTables;

namespace Core.Classes.Engine.Structs;

public class FStaticMeshSection
{
    // TODO change the serialization interfaces to use a "PackageStream" that has access to the package getting serialized so it can read objects directly
    public ObjectIndex Mat { get; set; }
    public int F10 { get; init; }
    public int F14 { get; init; }
    public int BEnableShadowCasting { get; init; }
    public int FirstIndex { get; init; }
    public int NumFaces { get; init; }
    public int F24 { get; init; }
    public int F28 { get; init; }
    public int Index { get; init; }
    public List<TwoInts> F30 { get; init; } = new();
    public byte Unk { get; init; }
}