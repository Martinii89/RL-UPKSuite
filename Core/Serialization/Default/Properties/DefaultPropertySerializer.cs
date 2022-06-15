using Core.Classes;
using Core.Classes.Core.Properties;
using Core.Flags;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Properties;

public class DefaultPropertySerializer : BaseObjectSerializer<UProperty>
{
    private readonly IObjectSerializer<UField> _fieldSerializer;
    private readonly IStreamSerializer<FName> _fnameSerializer;
    private readonly IStreamSerializer<ObjectIndex> _ObjectIndexSerializer;

    public DefaultPropertySerializer(IObjectSerializer<UField> fieldSerializer, IStreamSerializer<FName> fnameSerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        _fieldSerializer = fieldSerializer;
        _fnameSerializer = fnameSerializer;
        _ObjectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UProperty obj, IUnrealPackageStream objectStream)
    {
        _fieldSerializer.DeserializeObject(obj, objectStream);

        obj.ArrayDim = objectStream.ReadInt32();
        obj.PropertyFlags = objectStream.ReadUInt64();
        obj.Category = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream.BaseStream));
        obj.ArraySizeEnum = obj.OwnerPackage.GetObject(_ObjectIndexSerializer.Deserialize(objectStream.BaseStream)) as UEnum;

        if (obj.HasPropertyFlag(PropertyFlagsLO.Net))
        {
            obj.RepOffset = objectStream.ReadUInt16();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}