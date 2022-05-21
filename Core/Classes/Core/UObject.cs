using System.Text;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Classes.Core;

/// <summary>
///     The base for all unreal engine objects
/// </summary>
[NativeOnlyClass("Core", "Object")]
public class UObject
{
    /// <summary>
    ///     The FName of this object
    /// </summary>
    private readonly FName _name;

    private bool IsDeserialized;

    /// <summary>
    ///     Constructs a engine object
    /// </summary>
    /// <param name="name">The object name</param>
    /// <param name="class">The type of the object</param>
    /// <param name="outer">The parent</param>
    /// <param name="ownerPackage">The package where this object is defined</param>
    /// <param name="objectArchetype">The object template</param>
    public UObject(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
    {
        _name = name;
        Class = @class;
        Outer = outer;
        OwnerPackage = ownerPackage;
        ObjectArchetype = objectArchetype;
    }

    public IObjectSerializer? Serializer => Class?.GetInstanceSerializer();

    /// <summary>
    ///     The exportable item that was used to construct this object. May be null for unresolved import objects
    /// </summary>
    public ExportTableItem? ExportTableItem { get; set; }

    /// <summary>
    ///     The parent object
    /// </summary>
    public UObject? Outer { get; set; }

    /// <summary>
    ///     The name of this object
    /// </summary>
    public string Name => OwnerPackage.GetName(_name);

    /// <summary>
    ///     The type of this object
    /// </summary>
    public UClass? Class { get; set; }

    /// <summary>
    ///     The package where this object is defined
    /// </summary>
    public UnrealPackage OwnerPackage { get; init; }

    /// <summary>
    ///     The instance this object is based on. Values from the archetype will be coped over on construction
    /// </summary>
    public UObject? ObjectArchetype { get; init; }

    /// <summary>
    ///     Index related to network replication. From serial data
    /// </summary>
    public int NetIndex { get; set; }

    public List<FProperty> ScriptProperties { get; set; } = new();

    /// <summary>
    ///     Deserialize this object using the owner package data stream
    /// </summary>
    public void Deserialize()
    {
        // Can't deserialize without a export table item or a serializer 
        if (IsDeserialized || Serializer is null || ExportTableItem is null || OwnerPackage.PackageStream is null)
        {
            return;
        }

        var streamPosition = ExportTableItem.SerialOffset;
        OwnerPackage.PackageStream.Position = streamPosition;
        Serializer.DeserializeObject(this, OwnerPackage.PackageStream);
        IsDeserialized = true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(Name);
        var outer = Outer;
        while (outer != null)
        {
            var outerName = outer.Name;
            stringBuilder.Insert(0, ".");
            stringBuilder.Insert(0, outerName);
            outer = outer.Outer;
        }


        return stringBuilder.ToString();
    }
}