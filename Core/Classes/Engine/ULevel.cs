using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "LevelBase", typeof(UObject))]
public class ULevelBase : UObject
{
    public ULevelBase(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}

[NativeOnlyClass("Engine", "Level", typeof(ULevelBase))]
public class ULevel : ULevelBase
{
    public ULevel(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    [NativeProperty(PropertyType.FloatProperty)]
    public float ShadowmapTotalSize { get; set; }

    [NativeProperty(PropertyType.FloatProperty)]
    public float LightmapTotalSize { get; set; }
}