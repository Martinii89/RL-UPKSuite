using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Classes.Engine;
using Core.Types;

namespace Core.Classes;

/// <summary>
///     Base type for all unreal script objects with fields
/// </summary>
[NativeOnlyClass("Core", "Struct", typeof(UField))]
public class UStruct : UField
{
    /// <inheritdoc />
    public UStruct(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public Dictionary<string, UProperty> Properties { get; set; } = new();
    public UField? Children { get; set; }
    public UStruct? SuperStruct { get; set; }
    public UTextBuffer? ScriptText { get; set; }
    public UTextBuffer? CppText { get; set; }
    public int Line { get; set; }
    public int TextPos { get; set; }
    public int ScriptBytecodeSize { get; set; }
    public int DataScriptSize { get; set; }
    public long ScriptOffset { get; set; }
    public List<NativeProperty> NativeProperties { get; set; } = new();

    public void InitProperties()
    {
        Properties.Clear();
        foreach (var nativeProperty in NativeProperties)
        {
            var prop = nativeProperty.CreateProperty(this);
            if (prop is not null)
            {
                Properties.Add(prop.Name, prop);
            }
        }

        var properties = GetFieldIterator().OfType<UProperty>();
        foreach (var property in properties)
        {
            Properties.Add(property.Name, property);
        }
    }

    public UProperty? GetProperty(string propertyName)
    {
        if (!IsDeserialized)
        {
            Deserialize();
            InitProperties();
        }

        if (Properties.ContainsKey(propertyName))
        {
            return Properties[propertyName];
        }

        return SuperStruct?.GetProperty(propertyName);
    }

    /// <summary>
    ///     Iterate the properties
    /// </summary>
    /// <returns></returns>
    public IEnumerable<UProperty> GetPropertyIterator()
    {
        foreach (var uField in GetFieldIterator())
        {
            if (uField is UProperty property)
            {
                yield return property;
            }
        }
    }

    /// <summary>
    ///     Iterate the properties
    /// </summary>
    /// <returns></returns>
    public IEnumerable<UProperty> GetPropertyIteratorIncludingSupers()
    {
        if (SuperStruct is not null)
        {
            foreach (var superProperty in SuperStruct.GetPropertyIteratorIncludingSupers())
            {
                yield return superProperty;
            }
        }

        foreach (var uField in GetFieldIterator())
        {
            if (uField is UProperty property)
            {
                yield return property;
            }
        }
    }

    internal IEnumerable<UField> GetFieldIterator()
    {
        var field = Children;
        while (field is not null)
        {
            // If the field isn't deserialized, the next field will always be null.
            field.Deserialize();
            yield return field;
            field = field.Next as UField;
        }
    }
}