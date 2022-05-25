using Core.Classes;
using Core.Classes.Core;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object;

public class DefaultMetaDataSerializer : BaseObjectSerializer<UMetaData>
{
    private readonly IStreamSerializerFor<FName> _fnameSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultMetaDataSerializer(IStreamSerializerFor<ObjectIndex> objectIndexSerializer, IStreamSerializerFor<FName> fnameSerializer,
        IObjectSerializer<UObject> objectSerializer)
    {
        _objectIndexSerializer = objectIndexSerializer;
        _fnameSerializer = fnameSerializer;
        _objectSerializer = objectSerializer;
    }

    public override void DeserializeObject(UMetaData obj, Stream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        return;
        var numElements = objectStream.ReadInt32();
        for (var i = 0; i < numElements; i++)
        {
            var metaObj = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
            var count = objectStream.ReadInt32();
            UMetaData.MetaDataEntry data = new() { Object = metaObj };
            for (var j = 0; j < count; j++)
            {
                var name = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream));
                var nameValue = objectStream.ReadFString();
                data.Values.Add(new UMetaData.MetaDataEntry.MetaDataValue { key = name, value = nameValue });
            }

            obj.MetaData.Add(data);
        }
    }

    public override void SerializeObject(UMetaData obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}