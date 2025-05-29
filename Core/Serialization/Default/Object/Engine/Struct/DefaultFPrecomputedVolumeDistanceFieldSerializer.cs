using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;

namespace RlUpk.Core.Serialization.Default.Object.Engine.Struct;

public class DefaultFPrecomputedVolumeDistanceFieldSerializer : IStreamSerializer<FPrecomputedVolumeDistanceField>
{
    private readonly IStreamSerializer<FBox> _boxSerializer;

    private readonly IStreamSerializer<FColor> _colorSerializer;

    public DefaultFPrecomputedVolumeDistanceFieldSerializer(IStreamSerializer<FBox> boxSerializer, IStreamSerializer<FColor> colorSerializer)
    {
        _boxSerializer = boxSerializer;
        _colorSerializer = colorSerializer;
    }

    /// <inheritdoc />
    public FPrecomputedVolumeDistanceField Deserialize(Stream stream)
    {
        return new FPrecomputedVolumeDistanceField
        {
            VolumeMaxDistance = stream.ReadSingle(),
            VolumeBox = _boxSerializer.Deserialize(stream),
            VolumeSizeX = stream.ReadInt32(),
            VolumeSizeY = stream.ReadInt32(),
            VolumeSizeZ = stream.ReadInt32(),
            Colors = _colorSerializer.ReadTArrayToList(stream)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FPrecomputedVolumeDistanceField value)
    {
        stream.WriteSingle(value.VolumeMaxDistance);
        _boxSerializer.Serialize(stream, value.VolumeBox);
        stream.WriteInt32(value.VolumeSizeX);
        stream.WriteInt32(value.VolumeSizeY);
        stream.WriteInt32(value.VolumeSizeZ);
        _colorSerializer.WriteTArray(stream, value.Colors.ToArray());
    }
}