using Core.Classes.Engine.Structs;
using Core.Serialization.Extensions;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultStaticMeshSectionSerializer : IStreamSerializerFor<FStaticMeshSection>
{
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;

    public DefaultStaticMeshSectionSerializer(IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    {
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public FStaticMeshSection Deserialize(Stream stream)
    {
        return new FStaticMeshSection
        {
            Mat = _objectIndexSerializer.Deserialize(stream),
            F10 = stream.ReadInt32(),
            F14 = stream.ReadInt32(),
            BEnableShadowCasting = stream.ReadInt32(),
            FirstIndex = stream.ReadInt32(),
            NumFaces = stream.ReadInt32(),
            F24 = stream.ReadInt32(),
            F28 = stream.ReadInt32(),
            Index = stream.ReadInt32(),
            F30 = stream.ReadTarray(stream1 => new TwoInts
            {
                A = stream1.ReadInt32(),
                B = stream1.ReadInt32()
            }),
            Unk = (byte) stream.ReadByte()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FStaticMeshSection value)
    {
        throw new NotImplementedException();
    }
}