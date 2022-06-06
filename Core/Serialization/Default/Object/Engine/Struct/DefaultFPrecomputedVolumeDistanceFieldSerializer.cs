using Core.Classes.Core.Structs;
using Core.Classes.Engine;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultFPrecomputedVolumeDistanceFieldSerializer : IStreamSerializerFor<FPrecomputedVolumeDistanceField>
{
    private readonly IStreamSerializerFor<FBox> _boxSerializer;

    private readonly IStreamSerializerFor<FColor> _colorSerializer;

    public DefaultFPrecomputedVolumeDistanceFieldSerializer(IStreamSerializerFor<FBox> boxSerializer, IStreamSerializerFor<FColor> colorSerializer)
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