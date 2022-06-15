using Core.Classes;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object;

public class DefaultStateSerializer : BaseObjectSerializer<UState>
{
    private readonly IStreamSerializer<FName> _fnameSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UStruct> _structSerializer;

    public DefaultStateSerializer(IObjectSerializer<UStruct> structSerializer, IStreamSerializer<FName> fnameSerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        _structSerializer = structSerializer;
        _fnameSerializer = fnameSerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    public override void DeserializeObject(UState obj, IUnrealPackageStream objectStream)
    {
        _structSerializer.DeserializeObject(obj, objectStream);
        obj.ProbeMask = objectStream.ReadUInt32();
        obj.LabelTableOffset = objectStream.ReadUInt16();
        obj.StateFlags = objectStream.ReadUInt32();

        var funcCount = objectStream.ReadInt32();
        for (var i = 0; i < funcCount; i++)
        {
            var funcName = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream.BaseStream));
            if (obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream)) is UFunction func)
            {
                obj.FuncMap.Add(funcName, func);
            }
        }
    }

    public override void SerializeObject(UState obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}