using Core.Classes.Core.Structs;
using Core.Classes.Engine;
using Core.Serialization.Extensions;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultFURLSerialize : IStreamSerializer<FURL>
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

public class DefaultPrecomputedLightVolumeSerializer : IStreamSerializer<FPrecomputedLightVolume>
{
    private readonly IStreamSerializer<FBox> _boxSerializer;
    private readonly IStreamSerializer<FVolumeLightingSample> _volumeLightingSampleSerializer;

    public DefaultPrecomputedLightVolumeSerializer(IStreamSerializer<FBox> boxSerializer,
        IStreamSerializer<FVolumeLightingSample> volumeLightingSampleSerializer)
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