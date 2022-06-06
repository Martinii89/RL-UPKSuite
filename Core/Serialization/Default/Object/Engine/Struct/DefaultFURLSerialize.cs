using Core.Classes.Core.Structs;
using Core.Classes.Engine;
using Core.Serialization.Extensions;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultFURLSerialize : IStreamSerializerFor<FURL>
{
    public FURL Deserialize(Stream stream)
    {
        return new FURL
        {
            Protocol = stream.ReadFString(),
            Host = stream.ReadFString(),
            Map = stream.ReadFString(),
            Portal = stream.ReadFString(),
            Op = stream.ReadTarray(stream1 => stream1.ReadFString()),
            Port = stream.ReadInt32(),
            Valid = stream.ReadInt32()
        };
    }

    public void Serialize(Stream stream, FURL value)
    {
        throw new NotImplementedException();
    }
}

public class DefaultPrecomputedLightVolumeSerializer : IStreamSerializerFor<FPrecomputedLightVolume>
{
    private readonly IStreamSerializerFor<FBox> _boxSerializer;
    private readonly IStreamSerializerFor<FVolumeLightingSample> _volumeLightingSampleSerializer;

    public DefaultPrecomputedLightVolumeSerializer(IStreamSerializerFor<FBox> boxSerializer,
        IStreamSerializerFor<FVolumeLightingSample> volumeLightingSampleSerializer)
    {
        _boxSerializer = boxSerializer;
        _volumeLightingSampleSerializer = volumeLightingSampleSerializer;
    }

    public FPrecomputedLightVolume Deserialize(Stream stream)
    {
        var res = new FPrecomputedLightVolume();
        res.Initialized = stream.ReadInt32();
        if (res.Initialized != 1)
        {
            return res;
        }

        res.Bounds = _boxSerializer.Deserialize(stream);
        res.SampleSpacing = stream.ReadSingle();
        res.samples = _volumeLightingSampleSerializer.ReadTArrayToList(stream);
        return res;
    }

    public void Serialize(Stream stream, FPrecomputedLightVolume value)
    {
        throw new NotImplementedException();
    }
}