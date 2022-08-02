using Core.Classes.Engine.Structs;
using Core.Serialization.Extensions;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultKCachedConvexDataSerializer : IStreamSerializer<FKCachedConvexData>
{
    /// <inheritdoc />
    public FKCachedConvexData Deserialize(Stream stream)
    {
        return new FKCachedConvexData
        {
            CachedConvexElements = stream.ReadTarray(stream1 =>
            {
                var data = new FKCachedConvexDataElement
                {
                    ConvexElementData = stream1.ReadTarrayWithElementSize(stream2 => (byte) stream2.ReadByte())
                };
                return data;
            })
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FKCachedConvexData value)
    {
        stream.WriteTArray(value.CachedConvexElements,
            (stream1, element) => stream1.BulkWriteTArray(element.ConvexElementData, (stream2, b) => stream2.WriteByte(b)));
    }
}