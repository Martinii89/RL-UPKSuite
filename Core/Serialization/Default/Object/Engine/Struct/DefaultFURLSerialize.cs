using Core.Classes.Core.Structs;
using Core.Classes.Engine;
using Core.Serialization.Extensions;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultFURLSerialize : IStreamSerializer<FURL>
{
    /// <inheritdoc />
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

    /// <inheritdoc />
    public void Serialize(Stream stream, FURL value)
    {
        stream.WriteFString(value.Protocol);
        stream.WriteFString(value.Host);
        stream.WriteFString(value.Map);
        stream.WriteFString(value.Portal);
        stream.WriteTArray(value.Op, (stream1, s) => stream1.WriteFString(s));
        stream.WriteInt32(value.Port);
        stream.WriteInt32(value.Valid);
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

    /// <inheritdoc />
    public FPrecomputedLightVolume Deserialize(Stream stream)
    {
        var res = new FPrecomputedLightVolume
        {
            Initialized = stream.ReadInt32()
        };
        if (res.Initialized != 1)
        {
            return res;
        }

        res.Bounds = _boxSerializer.Deserialize(stream);
        res.SampleSpacing = stream.ReadSingle();
        res.samples = _volumeLightingSampleSerializer.ReadTArrayToList(stream);
        return res;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FPrecomputedLightVolume value)
    {
        stream.WriteInt32(value.Initialized);
        if (value.Initialized != 1)
        {
            return;
        }

        _boxSerializer.Serialize(stream, value.Bounds);
        stream.WriteSingle(value.SampleSpacing);
        _volumeLightingSampleSerializer.WriteTArray(stream, value.samples.ToArray());
    }
}