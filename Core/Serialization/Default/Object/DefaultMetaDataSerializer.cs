using RlUpk.Core.Classes.Core;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object;

public class DefaultMetaDataSerializer : BaseObjectSerializer<UMetaData>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultMetaDataSerializer(IObjectSerializer<UObject> objectSerializer)
    {
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
            var metaObj = objectStream.ReadObject();
            var count = objectStream.ReadInt32();
            UMetaData.MetaDataEntry data = new() { Object = metaObj };
            for (var j = 0; j < count; j++)
            {
                var name = objectStream.ReadFNameStr();
                var nameValue = objectStream.ReadFString();
                data.Values.Add(new UMetaData.MetaDataEntry.MetaDataValue { Key = name, Value = nameValue });
            }

            obj.MetaData.Add(data);
        }
    }

    public override void SerializeObject(UMetaData obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}