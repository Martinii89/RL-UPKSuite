using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultStaticMeshSectionSerializer : BaseObjectSerializer<FStaticMeshSection>
{
    /// <inheritdoc />
    public override void DeserializeObject(FStaticMeshSection obj, IUnrealPackageStream objectStream)
    {
        obj.Mat = objectStream.ReadObject() as UMaterialInterface;
        obj.F10 = objectStream.ReadInt32();
        obj.F14 = objectStream.ReadInt32();
        obj.BEnableShadowCasting = objectStream.ReadInt32();
        obj.FirstIndex = objectStream.ReadInt32();
        obj.NumFaces = objectStream.ReadInt32();
        obj.F24 = objectStream.ReadInt32();
        obj.F28 = objectStream.ReadInt32();
        obj.Index = objectStream.ReadInt32();
        obj.F30 = objectStream.ReadTArray(objectStream1 => new TwoInts
        {
            A = objectStream1.ReadInt32(),
            B = objectStream1.ReadInt32()
        });
        obj.Unk = objectStream.ReadByte();
    }

    /// <inheritdoc />
    public override void SerializeObject(FStaticMeshSection obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}