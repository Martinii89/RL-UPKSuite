namespace Core.Classes.Engine.Structs;

public class FkDOPTriangles
{
    public short[] Triangles { get; set; } = new short[4];

    //public void Deserialize(BinaryReader Reader)
    //{
    //    Triangles[0] = Reader.ReadInt16();
    //    Triangles[1] = Reader.ReadInt16();
    //    Triangles[2] = Reader.ReadInt16();
    //    Triangles[3] = Reader.ReadInt16();
    //}

    //public void Serialize(IUnrealStream writer)
    //{
    //    writer.Write(Triangles[0]);
    //    writer.Write(Triangles[1]);
    //    writer.Write(Triangles[2]);
    //    writer.Write(Triangles[3]);
    //}
}