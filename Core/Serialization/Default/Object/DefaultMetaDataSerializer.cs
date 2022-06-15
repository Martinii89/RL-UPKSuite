using Core.Classes;
using Core.Classes.Core;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object;

public class DefaultMetaDataSerializer : BaseObjectSerializer<UMetaData>
{
    private readonly IStreamSerializer<FName> _fnameSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultMetaDataSerializer(IStreamSerializer<ObjectIndex> objectIndexSerializer, IStreamSerializer<FName> fnameSerializer,
        IObjectSerializer<UObject> objectSerializer)
    {
        _objectIndexSerializer = objectIndexSerializer;
        _fnameSerializer = fnameSerializer;
        _objectSerializer = objectSerializer;
    }

    public override void DeserializeObject(UMetaData obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        var remaining = obj.ExportTableItem.SerialOffset + obj.ExportTableItem.SerialSize - objectStream.BaseStream.Position;
        objectStream.BaseStream.Move(remaining);
        return;
        var numElements = objectStream.ReadInt32();
        for (var i = 0; i < numElements; i++)
        {
            var metaObj = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream));
            var count = objectStream.ReadInt32();
            UMetaData.MetaDataEntry data = new() { Object = metaObj };
            for (var j = 0; j < count; j++)
            {
                var name = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream.BaseStream));
                var nameValue = objectStream.ReadFString();
                data.Values.Add(new UMetaData.MetaDataEntry.MetaDataValue { key = name, value = nameValue });
            }

            obj.MetaData.Add(data);
        }
    }

    public override void SerializeObject(UMetaData obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}