using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultMaterialInstanceSerializer : BaseObjectSerializer<UMaterialInstance>
{
    private readonly IObjectSerializer<FMaterialResource> _materialResourceSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IObjectSerializer<FStaticParameterSet> _staticParameterSetSerializer;

    public DefaultMaterialInstanceSerializer(IObjectSerializer<UObject> objectSerializer, IObjectSerializer<FMaterialResource> materialResourceSerializer,
        IObjectSerializer<FStaticParameterSet> staticParameterSetSerializer)
    {
        _objectSerializer = objectSerializer;
        _materialResourceSerializer = materialResourceSerializer;
        _staticParameterSetSerializer = staticParameterSetSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UMaterialInstance obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        var bHasStaticPermutationResource = obj.ScriptProperties.Find(x => x.Name == "bHasStaticPermutationResource");
        var valueAsBool = bHasStaticPermutationResource?.Value as byte?;
        if (valueAsBool != 1)
        {
            return;
        }

        obj.ResourceCountFlag = objectStream.ReadInt32();

        var count = obj.ResourceCountFlag switch
        {
            1 => 1,
            3 => 2,
            _ => throw new InvalidOperationException()
        };

        for (var i = 0; i < count; i++)
        {
            var res = new FMaterialResource();
            var paramSet = new FStaticParameterSet();
            _materialResourceSerializer.DeserializeObject(res, objectStream);
            _staticParameterSetSerializer.DeserializeObject(paramSet, objectStream);
            obj.StaticPermutationResource.Add(res);
            obj.StaticParameters.Add(paramSet);
        }

        DropRamainingNativeData(obj, objectStream.BaseStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(UMaterialInstance obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        var bHasStaticPermutationResource = obj.ScriptProperties.Find(x => x.Name == "bHasStaticPermutationResource");
        var valueAsBool = bHasStaticPermutationResource?.Value as byte?;
        if (valueAsBool != 1)
        {
            return;
        }

        objectStream.WriteInt32(obj.ResourceCountFlag);
        var count = obj.ResourceCountFlag switch
        {
            1 => 1,
            3 => 2,
            _ => throw new InvalidOperationException()
        };
        for (var i = 0; i < count; i++)
        {
            _materialResourceSerializer.SerializeObject(obj.StaticPermutationResource[i], objectStream);
            _staticParameterSetSerializer.SerializeObject(obj.StaticParameters[i], objectStream);
        }
    }
}