using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Core.Classes.Core.Properties;
using Core.Flags;
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
    protected FName _FName;

    protected bool IsDeserialized;

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
        _FName = name;
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
    public string Name => OwnerPackage.GetName(_FName);

    /// <summary>
    ///     The type of this object
    /// </summary>
    public UClass? Class { get; set; }

    /// <summary>
    ///     The package where this object is defined
    /// </summary>
    public UnrealPackage OwnerPackage { get; protected set; }

    /// <summary>
    ///     The instance this object is based on. Values from the archetype will be coped over on construction
    /// </summary>
    public UObject? ObjectArchetype { get; init; }

    /// <summary>
    ///     Index related to network replication. From serial data
    /// </summary>
    public int NetIndex { get; set; }

    public List<FProperty> ScriptProperties { get; set; } = new();

    public ulong ObjectFlags => ExportTableItem?.ObjectFlags ?? 0L;

    public bool IsDefaultObject => (ObjectFlags & 0x200) != 0;

    public bool IsArchetypeObject => (ObjectFlags & 0x400) != 0;

    private IUnrealPackageStream? OwnerPackageStream => OwnerPackage.UnrealPackageStream;

    public long DeserializationOffsetEnd { get; set; }

    public bool FullyDeserialized { get; set; }


    [MemberNotNullWhen(true, nameof(ExportTableItem))]
    [MemberNotNullWhen(true, nameof(OwnerPackageStream))]
    [MemberNotNullWhen(true, nameof(Serializer))]
    internal bool CanDeserialize()
    {
        return !IsDeserialized && Serializer is not null && ExportTableItem is not null && ExportTableItem.SerialSize != 0 && OwnerPackageStream is not null;
    }

    /// <summary>
    ///     Deserialize this object using the owner package data stream
    ///     NOTE: UObject::Serialize has as vtable index of 13 (in rocket league)
    /// </summary>
    public void Deserialize()
    {
        if (!CanDeserialize())
        {
            return;
        }

        var streamPosition = ExportTableItem.SerialOffset;
        using var rewindAfterScope = OwnerPackageStream.TemporarySeek();
        OwnerPackageStream.BaseStream.Position = streamPosition;

        Serializer.DeserializeObject(this, OwnerPackageStream);

        // Store meta data about the deserialization for manual debugging purposes
        DeserializationOffsetEnd = OwnerPackageStream.BaseStream.Position - ExportTableItem!.SerialOffset;
        FullyDeserialized = OwnerPackageStream.BaseStream.Position == ExportTableItem!.SerialOffset + ExportTableItem.SerialSize;
        if (!FullyDeserialized)
        {
            Debugger.Break();
        }

        IsDeserialized = true;
    }

    public bool HasObjectFlag(ObjectFlagsLO flag)
    {
        return ((ObjectFlags >> 32) & (uint) flag) != 0;
    }

    /// <summary>
    ///     Enumerator for the outer chain
    /// </summary>
    /// <returns></returns>
    public IEnumerable<UObject> GetOuterEnumerable()
    {
        var outer = Outer;
        while (outer != null)
        {
            yield return outer;
            outer = outer.Outer;
        }
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