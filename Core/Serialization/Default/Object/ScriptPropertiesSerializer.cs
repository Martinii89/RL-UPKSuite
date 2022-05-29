using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Extensions;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object;

public class ScriptPropertiesSerializer
{
    private readonly IStreamSerializerFor<FName> _fnameSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;

    public ScriptPropertiesSerializer(IStreamSerializerFor<FName> fnameSerializer, IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    {
        _fnameSerializer = fnameSerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    private void ReadIntProperty(UObject obj, Stream stream, FProperty property)
    {
        property.ValueStart = stream.Position;
        property.Value = stream.ReadInt32();
    }

    private void ReadStrProperty(Stream stream, FProperty property)
    {
        property.ValueStart = stream.Position;
        property.Value = stream.ReadFString();
    }

    private void ReadBoolProperty(Stream stream, FProperty property)
    {
        property.ValueStart = stream.Position;
        property.Value = stream.ReadByte() == 1;
    }

    private void ReadByteProperty(Stream stream, FProperty property)
    {
        property.EnumName = property.Package.GetName(_fnameSerializer.Deserialize(stream));
        property.ValueStart = stream.Position;
        property.Value = property.Package.GetName(_fnameSerializer.Deserialize(stream));
    }


    private void ReadFloatProperty(Stream stream, FProperty property)
    {
        property.ValueStart = stream.Position;
        property.Value = stream.ReadSingle();
    }

    private void ReadObjectProperty(Stream stream, FProperty property)
    {
        property.ValueStart = stream.Position;
        property.Value = property.Package.GetObject(_objectIndexSerializer.Deserialize(stream));
    }

    private void ReadStructProperty(Stream stream, FProperty property)
    {
        property.StructName = property.Package.GetName(_fnameSerializer.Deserialize(stream));
        property.ValueStart = stream.Position;
        stream.Move(property.Size);
    }

    private void ReadNameProperty(Stream stream, FProperty property)
    {
        property.ValueStart = stream.Position;
        property.Value = property.Package.GetName(_fnameSerializer.Deserialize(stream));
    }

    public IEnumerable<FProperty> GetScriptProperties(UObject obj, Stream objectStream)
    {
        var objClass = obj.Class;
        if (objClass is null)
        {
            Debugger.Break();
        }

        while (true)
        {
            var fName = _fnameSerializer.Deserialize(objectStream);
            var name = obj.OwnerPackage.GetName(fName);

            if (name == "None")
            {
                yield break;
            }

            var strucUProperty = objClass?.GetProperty(name);

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

            property.Value = strucUProperty.DeserializeValue(obj, objectStream, property.Size, _fnameSerializer, _objectIndexSerializer);

            //switch (property.Type)
            //{
            //    case PropertyType.BoolProperty:
            //        ReadBoolProperty(objectStream, property);
            //        break;
            //    case PropertyType.StructProperty:
            //        ReadStructProperty(objectStream, property);
            //        break;
            //    case PropertyType.ByteProperty:
            //        ReadByteProperty(objectStream, property);
            //        break;
            //    case PropertyType.IntProperty:
            //        ReadIntProperty(obj, objectStream, property);
            //        break;
            //    case PropertyType.FloatProperty:
            //        ReadFloatProperty(objectStream, property);
            //        break;
            //    case PropertyType.ObjectProperty:
            //        ReadObjectProperty(objectStream, property);
            //        break;
            //    case PropertyType.NameProperty:
            //        ReadNameProperty(objectStream, property);
            //        break;
            //    case PropertyType.StrProperty:
            //        ;                    ReadStrProperty(objectStream, property);
            //        break;
            //    case PropertyType.ArrayProperty:
            //    case PropertyType.ClassProperty:
            //    case PropertyType.QWordProperty:
            //    case PropertyType.InterfaceProperty:
            //    case PropertyType.ComponentProperty:

            //    case PropertyType.None:
            //    case PropertyType.StringProperty:
            //    case PropertyType.MapProperty:
            //    case PropertyType.FixedArrayProperty:
            //    case PropertyType.XWeakReferenceProperty:
            //    case PropertyType.PointerProperty:
            //    case PropertyType.StructOffset:
            //    case PropertyType.Vector:
            //    case PropertyType.Rotator:
            //    case PropertyType.Color:
            //    case PropertyType.Vector2D:
            //    case PropertyType.Vector4:
            //    case PropertyType.Guid:
            //    case PropertyType.Plane:
            //    case PropertyType.Sphere:
            //    case PropertyType.Scale:
            //    case PropertyType.Box:
            //    case PropertyType.Quat:
            //    case PropertyType.Matrix:
            //    case PropertyType.LinearColor:
            //    case PropertyType.IntPoint:
            //    case PropertyType.TwoVectors:
            //    default:
            //        property.ValueStart = objectStream.Position;
            //        objectStream.Move(property.Size);
            //        break;
            //}

            yield return property;
        }
    }
}