using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Core;

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
            public string Key { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
        }
    }
}