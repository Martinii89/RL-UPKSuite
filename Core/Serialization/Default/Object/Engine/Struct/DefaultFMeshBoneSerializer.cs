using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;
using Core.Types;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultFMeshBoneSerializer : IStreamSerializer<FMeshBone>
{
    private readonly IStreamSerializer<FColor> _colorSerializer;
    private readonly IStreamSerializer<FName> _nameSerializer;
    private readonly IStreamSerializer<FQuat> _quatSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;


    public DefaultFMeshBoneSerializer(IStreamSerializer<FColor> colorSerializer, IStreamSerializer<FQuat> quatSerializer,
        IStreamSerializer<FVector> vectorSerializer, IStreamSerializer<FName> nameSerializer)
    {
        _colorSerializer = colorSerializer;
        _quatSerializer = quatSerializer;
        _vectorSerializer = vectorSerializer;
        _nameSerializer = nameSerializer;
    }

    /// <inheritdoc />
    public FMeshBone Deserialize(Stream stream)
    {
        return new FMeshBone
        {
            Name = _nameSerializer.Deserialize(stream),
            Flags = stream.ReadUInt32(),
            BonePos = new VJointPos { Position = _vectorSerializer.Deserialize(stream), Orientation = _quatSerializer.Deserialize(stream) },
            NumChildren = stream.ReadInt32(),
            ParentIndex = stream.ReadInt32(),
            BoneColor = _colorSerializer.Deserialize(stream)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FMeshBone value)
    {
        throw new NotImplementedException();
    }
}