namespace Core.Classes.Engine.Structs;

public class FStaticMeshSection
{
    // TODO change the serialization interfaces to use a "PackageStream" that has access to the package getting serialized so it can read objects directly
    public UMaterialInterface? Mat { get; set; }
    public int F10 { get; set; }
    public int F14 { get; set; }
    public int BEnableShadowCasting { get; set; }
    public int FirstIndex { get; set; }
    public int NumFaces { get; set; }
    public int F24 { get; set; }
    public int F28 { get; set; }
    public int Index { get; set; }
    public List<TwoInts> F30 { get; set; } = new();
    public byte Unk { get; set; }
}