﻿using System.Text;
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

    public IObjectSerializer<UObject>? Serializer { get; set; }

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

    public void Deserialize(Stream stream)
    {
        // Can't deserialize without a export table item or a serializer 
        if (Serializer is null || ExportTableItem is null)
        {
            return;
        }

        Serializer.DeserializeObject(this, stream);
    }

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