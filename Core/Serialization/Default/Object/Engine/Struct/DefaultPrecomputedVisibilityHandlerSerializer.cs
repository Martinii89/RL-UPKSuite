﻿using System.Diagnostics;
using Core.Classes.Core.Structs;
using Core.Classes.Engine;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultPrecomputedVisibilityHandlerSerializer : IStreamSerializerFor<FPrecomputedVisibilityHandler>
{
    private readonly IStreamSerializerFor<FVector2D> _vector2dSerializer;

    public DefaultPrecomputedVisibilityHandlerSerializer(IStreamSerializerFor<FVector2D> vector2dSerializer)
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
        throw new NotImplementedException();
    }
}