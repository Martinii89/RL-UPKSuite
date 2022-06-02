namespace Core.Classes.Engine.Structs;

public class UvItem
{
    public UvItem()
    {
    }

    public UvItem(UvHalf[] uv)
    {
        Uv = uv;
    }

    public uint N0 { get; set; }
    public uint N1 { get; set; }
    public UvHalf[] Uv { get; }

    //public void Deserialize(BinaryReader Reader)
    //{
    //    N0.Deserialize(Reader);
    //    N1.Deserialize(Reader);
    //    for (var i = 0; i < Uv.Length; i++)
    //    {
    //        Uv[i] = new UvHalf();
    //        Uv[i].Deserialize(Reader);
    //    }
    //}

    //public void Serialize(IUnrealStream writer)
    //{
    //    N0.Serialize(writer);
    //    N1.Serialize(writer);
    //    foreach (var uvHalf in Uv)
    //    {
    //        uvHalf.Serialize(writer);
    //    }
    //}
}