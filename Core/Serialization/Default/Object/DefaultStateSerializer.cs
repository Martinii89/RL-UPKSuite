using Core.Classes;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object;

public class DefaultStateSerializer : BaseObjectSerializer<UState>
{
    private readonly IObjectSerializer<UStruct> _structSerializer;

    public DefaultStateSerializer(IObjectSerializer<UStruct> structSerializer)
    {
        _structSerializer = structSerializer;
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
            var funcName = objectStream.ReadFNameStr();
            if (objectStream.ReadObject() is UFunction func)
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