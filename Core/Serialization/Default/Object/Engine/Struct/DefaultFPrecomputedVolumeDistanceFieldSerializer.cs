using Core.Classes.Core.Structs;
using Core.Classes.Engine;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultFPrecomputedVolumeDistanceFieldSerializer : IStreamSerializer<FPrecomputedVolumeDistanceField>
{
    private readonly IStreamSerializer<FBox> _boxSerializer;

    private readonly IStreamSerializer<FColor> _colorSerializer;

    public DefaultFPrecomputedVolumeDistanceFieldSerializer(IStreamSerializer<FBox> boxSerializer, IStreamSerializer<FColor> colorSerializer)
    {
        _boxSerializer = boxSerializer;
        _colorSerializer = colorSerializer;
    }

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

    public void Serialize(Stream stream, FPrecomputedVolumeDistanceField value)
    {
        throw new NotImplementedException();
    }
}