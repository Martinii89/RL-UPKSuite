using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Flags;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object;

/// <summary>
///     Default implementation for a UObject serializer
/// </summary>
public class DefaultObjectSerializer : BaseObjectSerializer<UObject>
{
    private readonly IStreamSerializerFor<FName> _fnameSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;
    private readonly ScriptPropertiesSerializer _scriptPropertiesSerializer;

    public DefaultObjectSerializer(IStreamSerializerFor<FName> fnameSerializer, IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    {
        _fnameSerializer = fnameSerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _scriptPropertiesSerializer = new ScriptPropertiesSerializer(fnameSerializer, objectIndexSerializer);
    }

    /// <inheritdoc />
    public override void DeserializeObject(UObject obj, Stream objectStream)
    {
        if (obj.HasObjectFlag(ObjectFlagsLO.HasStack))
        {
            var node = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
            var stateNode = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
            var probeMask = objectStream.ReadUInt32();
            var latentAction = objectStream.ReadUInt16();
            var stateStackCount = objectStream.ReadUInt32();
            var offset = objectStream.ReadInt32();
        }

        obj.NetIndex = objectStream.ReadInt32();

        if (obj.Class == UClass.StaticClass)
        {
            return;
        }

        //obj.ScriptProperties.AddRange(GetScriptProperties(obj, objectStream));
        obj.ScriptProperties.AddRange(_scriptPropertiesSerializer.GetScriptProperties(obj, objectStream));
    }

    private IEnumerable<FProperty> GetScriptProperties(UObject obj, Stream objectStream)
    {
        while (true)
        {
            var fName = _fnameSerializer.Deserialize(objectStream);
            var name = obj.OwnerPackage.GetName(fName);

            if (name == "None")
            {
                yield break;
            }

            var typeFName = _fnameSerializer.Deserialize(objectStream);
            var propType = Enum.Parse<PropertyType>(obj.OwnerPackage.GetName(typeFName));

            FProperty property = new()
            {
                Package = obj.OwnerPackage,
                Name = name,
                Type = propType,
                Size = objectStream.ReadInt32(),
                ArrayIndex = objectStream.ReadInt32()
            };
            switch (property.Type)
            {
                case PropertyType.BoolProperty:
                    objectStream.Move(1);
                    break;
                case PropertyType.StructProperty:
                    property.StructName = property.Package.GetName(_fnameSerializer.Deserialize(objectStream));
                    break;
                case PropertyType.ByteProperty:
                    property.EnumName = property.Package.GetName(_fnameSerializer.Deserialize(objectStream));
                    break;
            }

            property.ValueStart = objectStream.Position;
            objectStream.Move(property.Size);


            yield return property;
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UObject obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}