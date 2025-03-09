using Core.Classes;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object;

/// <summary>
///     Default implementation for a UStruct serializer
/// </summary>
public class DefaultStructSerializer : BaseObjectSerializer<UStruct>
{
    private readonly IObjectSerializer<UField> _fieldSerializer;

    /// <summary>
    ///     Construct a DefaultStructSerializer with the required field serializers
    /// </summary>
    /// <param name="fieldSerializer"></param>
    public DefaultStructSerializer(IObjectSerializer<UField> fieldSerializer)
    {
        _fieldSerializer = fieldSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UStruct obj, IUnrealPackageStream objectStream)
    {
        _fieldSerializer.DeserializeObject(obj, objectStream);

        obj.SuperStruct = objectStream.ReadObject() as UStruct;
        obj.ScriptText = objectStream.ReadObject() as UTextBuffer;
        obj.Children = objectStream.ReadObject() as UField;
        obj.CppText = objectStream.ReadObject() as UTextBuffer;
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