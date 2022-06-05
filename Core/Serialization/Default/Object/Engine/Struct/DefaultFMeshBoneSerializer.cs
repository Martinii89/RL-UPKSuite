using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;
using Core.Types;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultFMeshBoneSerializer : IStreamSerializerFor<FMeshBone>
{
    private readonly IStreamSerializerFor<FColor> _colorSerializer;
    private readonly IStreamSerializerFor<FName> _nameSerializer;
    private readonly IStreamSerializerFor<FQuat> _quatSerializer;
    private readonly IStreamSerializerFor<FVector> _vectorSerializer;


    public DefaultFMeshBoneSerializer(IStreamSerializerFor<FColor> colorSerializer, IStreamSerializerFor<FQuat> quatSerializer,
        IStreamSerializerFor<FVector> vectorSerializer, IStreamSerializerFor<FName> nameSerializer)
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