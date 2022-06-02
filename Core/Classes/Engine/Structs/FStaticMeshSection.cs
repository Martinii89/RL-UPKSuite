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

    //public void Deserialize(BinaryReader Reader)
    //{
    //    if (Reader is UnrealReader r)
    //    {
    //        var package = r.GetPackage();
    //        Mat = package.Stream.ReadObject();
    //    }
    //    else
    //    {
    //        MatIndex = Reader.ReadInt32();
    //    }

    //    F10 = Reader.ReadInt32();
    //    F14 = Reader.ReadInt32();
    //    BEnableShadowCasting = Reader.ReadInt32();
    //    FirstIndex = Reader.ReadInt32();
    //    NumFaces = Reader.ReadInt32();
    //    F24 = Reader.ReadInt32();
    //    F28 = Reader.ReadInt32();
    //    Index = Reader.ReadInt32();
    //    F30.Deserialize(Reader);
    //    Unk = Reader.ReadByte();
    //}

    //public void Serialize(IUnrealStream writer)
    //{
    //    writer.Write((int) 0); // Mat
    //    writer.Write(F10);
    //    writer.Write(F14);
    //    writer.Write(BEnableShadowCasting);
    //    writer.Write(FirstIndex);
    //    writer.Write(NumFaces);
    //    writer.Write(F24);
    //    writer.Write(F28);
    //    writer.Write(Index);
    //    F30.Serialize(writer);
    //    writer.Write(Unk);
    //}
}