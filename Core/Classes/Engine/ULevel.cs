using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;
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

    public FURL URL { get; set; }

    public TransientArray<UObject> Actors { get; set; } = new();
    public FURL Url { get; set; } = new();
    public UObject? Model { get; set; }
    public List<UObject?> ModelComponents { get; set; } = new();
    public List<UObject?> GameSequences { get; set; } = new();
    public Dictionary<UTexture, List<FStreamableTextureInstance>> TextureToInstancesMap { get; set; } = new();
    public Dictionary<UComponent, List<FDynamicTextureInstance>> DynamicTextureInstances { get; set; } = new();
    public TArray<byte> CachedPhysBSPData { get; set; } = new();
    public Dictionary<UStaticMesh, FCachedPhysSMData> CachedPhysSMDataMap { get; set; }
    public List<FKCachedConvexData> CachedPhysSMDataStore { get; set; }
}

public class FCachedPhysSMData
{
    public FVector Scale3d { get; set; } = new();
    public int CachedDataIndex { get; set; }
}

public class FStreamableTextureInstance
{
    public FVector Center { get; set; } = new();
    public int W { get; set; }
    public float TexelFactor { get; set; }
}

public class FDynamicTextureInstance : FStreamableTextureInstance
{
    public UTexture? Tex { get; set; }
    public int BAttached { get; set; }
    public float OriginalRadius { get; set; }
}

public class FURL
{
    public string Protocol { get; set; }
    public string Host { get; set; }
    public string Map { get; set; }
    public string Portal { get; set; }
    public List<string> Op { get; set; } = new();
    public int Port { get; set; }
    public int Valid { get; set; }
}

public class TransientArray<T>
{
    public T? Super { get; set; }
    public List<T?> Data { get; set; } = new();
}