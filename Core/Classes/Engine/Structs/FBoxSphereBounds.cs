using Core.Classes.Core.Structs;

namespace Core.Classes.Engine.Structs;

public class FBoxSphereBounds
{
    public FVector Origin { get; set; } = new();
    public FVector BoxExtent { get; set; } = new();
    public float SphereRadius { get; set; }

    //public void Deserialize(BinaryReader Reader)
    //{
    //    Origin.Deserialize(Reader);
    //    BoxExtent.Deserialize(Reader);
    //    SphereRadius = Reader.ReadSingle();
    //}

    //public void Serialize(IUnrealStream writer)
    //{
    //    Origin.Serialize(writer);
    //    BoxExtent.Serialize(writer);
    //    writer.Write(SphereRadius);
    //}
}