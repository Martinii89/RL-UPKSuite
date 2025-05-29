using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Properties;
using RlUpk.Core.Flags;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class RLPropertySerializer : BaseObjectSerializer<UProperty>
{
    private readonly IObjectSerializer<UField> _fieldSerializer;

    public RLPropertySerializer(IObjectSerializer<UField> fieldSerializer)
    {
        _fieldSerializer = fieldSerializer;
    }

    public override void DeserializeObject(UProperty obj, IUnrealPackageStream objectStream)
    {
        _fieldSerializer.DeserializeObject(obj, objectStream);

        obj.ArrayDim = objectStream.ReadInt32();
        obj.PropertyFlags = objectStream.ReadUInt64();
        obj.Category = objectStream.ReadFNameStr();
        obj.ArraySizeEnum = objectStream.ReadObject() as UEnum;

        if (obj.HasPropertyFlag(PropertyFlagsLO.Net))
        {
            obj.RepOffset = objectStream.ReadUInt16();
        }


        var name = objectStream.ReadFString();
    }

    public override void SerializeObject(UProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}