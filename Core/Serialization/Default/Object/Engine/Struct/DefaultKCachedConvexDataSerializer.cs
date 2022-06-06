using Core.Classes.Engine.Structs;
using Core.Serialization.Extensions;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultKCachedConvexDataSerializer : IStreamSerializerFor<FKCachedConvexData>
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
        throw new NotImplementedException();
    }
}