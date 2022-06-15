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

    public void Serialize(Stream stream, FVolumeLightingSample value)
    {
        throw new NotImplementedException();
    }
}