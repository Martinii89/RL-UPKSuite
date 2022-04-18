namespace Core.Serialization;

public static class GenericSerializer
{
    public static object Deserialize(object o, BinaryReader reader)
    {
        switch (o)
        {
            case IBinaryDeserializableClass serializable:
                serializable.Deserialize(reader);
                return o;
            case int:
                return reader.ReadInt32();
            case ushort:
                return reader.ReadUInt16();
            default:
                throw new NotImplementedException();
        }
    }
}