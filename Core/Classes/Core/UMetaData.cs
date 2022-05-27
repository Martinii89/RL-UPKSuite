using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

/// <summary>
///     Holds a TMap of key\value pairs
/// </summary>
[NativeOnlyClass("Core", "MetaData", typeof(UObject))]
public class UMetaData : UObject
{
    /// <inheritdoc />
    public UMetaData(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public List<MetaDataEntry> MetaData { get; set; } = new();

    public class MetaDataEntry
    {
        public UObject Object { get; set; }

        public List<MetaDataValue> Values { get; set; } = new();

        public class MetaDataValue
        {
            public string key { get; set; }
            public string value { get; set; }
        }
    }
}