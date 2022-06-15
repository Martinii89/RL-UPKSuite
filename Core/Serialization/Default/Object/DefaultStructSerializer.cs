using Core.Classes;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object;

/// <summary>
///     Default implementation for a UStruct serializer
/// </summary>
public class DefaultStructSerializer : BaseObjectSerializer<UStruct>
{
    private readonly IObjectSerializer<UField> _fieldSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerialiser;

    /// <summary>
    ///     Construct a DefaultStructSerializer with the required field serializers
    /// </summary>
    /// <param name="fieldSerializer"></param>
    /// <param name="objectIndexSerialiser"></param>
    public DefaultStructSerializer(IObjectSerializer<UField> fieldSerializer, IStreamSerializer<ObjectIndex> objectIndexSerialiser)
    {
        _fieldSerializer = fieldSerializer;
        _objectIndexSerialiser = objectIndexSerialiser;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UStruct obj, IUnrealPackageStream objectStream)
    {
        _fieldSerializer.DeserializeObject(obj, objectStream);

        obj.SuperStruct = obj.OwnerPackage.GetObject(_objectIndexSerialiser.Deserialize(objectStream.BaseStream)) as UStruct;
        obj.ScriptText = obj.OwnerPackage.GetObject(_objectIndexSerialiser.Deserialize(objectStream.BaseStream)) as UTextBuffer;
        obj.Children = obj.OwnerPackage.GetObject(_objectIndexSerialiser.Deserialize(objectStream.BaseStream)) as UField;
        obj.CppText = obj.OwnerPackage.GetObject(_objectIndexSerialiser.Deserialize(objectStream.BaseStream)) as UTextBuffer;
        obj.Line = objectStream.ReadInt32();
        obj.TextPos = objectStream.ReadInt32();
        obj.ScriptBytecodeSize = objectStream.ReadInt32();
        obj.DataScriptSize = objectStream.ReadInt32();
        if (obj.DataScriptSize > 0)
        {
            obj.ScriptOffset = objectStream.BaseStream.Position;
            objectStream.BaseStream.Move(obj.DataScriptSize);
        }


        obj.InitProperties();
    }

    /// <inheritdoc />
    public override void SerializeObject(UStruct obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}