﻿using Core.Classes.Core.Structs;

namespace Core.Serialization.Default.Object.Core.Struct;

public class DefaultFColorStreamSerializer : IStreamSerializerFor<FColor>
{
    /// <inheritdoc />
    public FColor Deserialize(Stream stream)
    {
        return new FColor
        {
            R = (byte) stream.ReadByte(),
            G = (byte) stream.ReadByte(),
            B = (byte) stream.ReadByte(),
            A = (byte) stream.ReadByte()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FColor value)
    {
        throw new NotImplementedException();
    }
}