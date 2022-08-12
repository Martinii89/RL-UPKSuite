using Core.Classes.Core.Structs;
using Core.Classes.Engine;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultVolumeLightingSampleSerializer : IStreamSerializer<FVolumeLightingSample>
{
    private readonly IStreamSerializer<FColor> _colorSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultVolumeLightingSampleSerializer(IStreamSerializer<FColor> colorSerializer, IStreamSerializer<FVector> vectorSerializer)
    {
        _colorSerializer = colorSerializer;
        _vectorSerializer = vectorSerializer;
    }


    /// <inheritdoc />
    public FVolumeLightingSample Deserialize(Stream stream)
    {
        return new FVolumeLightingSample
        {
            Position = _vectorSerializer.Deserialize(stream),
            Radius = stream.ReadSingle(),
            IndirectDirectionTheta = (byte) stream.ReadByte(),
            IndirectDirectionPhi = (byte) stream.ReadByte(),
            EnvironmentDirectionTheta = (byte) stream.ReadByte(),
            EnvironmentDirectionPhi = (byte) stream.ReadByte(),
            IndirectRadiance = _colorSerializer.Deserialize(stream),
            EnvironmentRadiance = _colorSerializer.Deserialize(stream),
            AmbientRadiance = _colorSerializer.Deserialize(stream),
            bShadowedFromDominantLights = (byte) stream.ReadByte()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FVolumeLightingSample value)
    {
        _vectorSerializer.Serialize(stream, value.Position);
        stream.WriteSingle(value.Radius);
        stream.WriteByte(value.IndirectDirectionTheta);
        stream.WriteByte(value.IndirectDirectionPhi);
        stream.WriteByte(value.EnvironmentDirectionTheta);
        stream.WriteByte(value.EnvironmentDirectionPhi);
        _colorSerializer.Serialize(stream, value.IndirectRadiance);
        _colorSerializer.Serialize(stream, value.EnvironmentRadiance);
        _colorSerializer.Serialize(stream, value.AmbientRadiance);
        stream.WriteByte(value.bShadowedFromDominantLights);
    }
}