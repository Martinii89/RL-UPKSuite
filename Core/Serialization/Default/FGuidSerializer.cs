using System.Buffers.Binary;

using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types;

namespace RlUpk.Core.Serialization.Default;

/// <inheritdoc />
public class FGuidSerializer : IStreamSerializer<FGuid>
{
    /// <inheritdoc />
    public FGuid Deserialize(Stream stream)
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint) * 4];
        stream.ReadExactly(buffer);
        return new FGuid
        {
            A = BinaryPrimitives.ReadUInt32LittleEndian(buffer),
            B = BinaryPrimitives.ReadUInt32LittleEndian(buffer[4..]),
            C = BinaryPrimitives.ReadUInt32LittleEndian(buffer[8..]),
            D =  BinaryPrimitives.ReadUInt32LittleEndian(buffer[12..])
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FGuid value)
    {
        stream.WriteUInt32(value.A);
        stream.WriteUInt32(value.B);
        stream.WriteUInt32(value.C);
        stream.WriteUInt32(value.D);
    }
}