using System.Diagnostics;

using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Engine.Struct;

public class DefaultPrecomputedVisibilityHandlerSerializer : IStreamSerializer<FPrecomputedVisibilityHandler>
{
    private readonly IStreamSerializer<FVector2D> _vector2dSerializer;

    public DefaultPrecomputedVisibilityHandlerSerializer(IStreamSerializer<FVector2D> vector2dSerializer)
    {
        _vector2dSerializer = vector2dSerializer;
    }

    /// <inheritdoc />
    public FPrecomputedVisibilityHandler Deserialize(Stream stream)
    {
        var res = new FPrecomputedVisibilityHandler
        {
            PrecomputedVisibilityCellBucketOriginXY = _vector2dSerializer.Deserialize(stream),
            PrecomputedVisibilityCellSizeXY = stream.ReadSingle(),
            PrecomputedVisibilityCellSizeZ = stream.ReadSingle(),
            PrecomputedVisibilityCellBucketSizeXY = stream.ReadInt32(),
            PrecomputedVisibilityNumCellBuckets = stream.ReadInt32()
        };
        var precomputedVisibilityCellBucketsCount = stream.ReadInt32();
        if (precomputedVisibilityCellBucketsCount > 0)
        {
            Debugger.Break();
        }

        return res;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FPrecomputedVisibilityHandler value)
    {
        _vector2dSerializer.Serialize(stream, value.PrecomputedVisibilityCellBucketOriginXY);
        stream.WriteSingle(value.PrecomputedVisibilityCellSizeXY);
        stream.WriteSingle(value.PrecomputedVisibilityCellSizeZ);
        stream.WriteInt32(value.PrecomputedVisibilityCellBucketSizeXY);
        stream.WriteInt32(value.PrecomputedVisibilityNumCellBuckets);
        stream.WriteInt32(0); // precomputedVisibilityCellBucketsCount
    }
}