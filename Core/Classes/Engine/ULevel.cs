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
    public TMultiMap<UStaticMesh, FCachedPhysSMData> CachedPhysSMDataMap { get; set; } = new();
    public List<FKCachedConvexData> CachedPhysSMDataStore { get; set; } = new();
    public TMultiMap<UStaticMesh, FCachedPerTriPhysSMData> CachedPhysPerTriSMDataMap { get; set; } = new();
    public List<FKCachedPerTriData> CachedPhysPerTriSMDataStore { get; set; } = new();
    public int CachedPhysBSPDataVersion { get; set; }
    public int CachedPhysSMDataVersion { get; set; }
    public Dictionary<UTexture, bool> ForceStreamTextures { get; set; } = new();
    public List<FKCachedConvexDataElement> CachedPhysConvexBSPData { get; set; } = new();
    public int CachedPhysConvexBSPVersion { get; set; }

    public UObject? NavListStart { get; set; }
    public UObject? NavListEnd { get; set; }
    public UObject? CoverListStart { get; set; }
    public UObject? CoverListEnd { get; set; }
    public UObject? PylonListStart { get; set; }
    public UObject? PylonListEnd { get; set; }
    public int UnkArrayCountOf20Bytes { get; set; }
    public List<UObject?> SomeObjectArray { get; set; } = new();
    public List<Tuple<UObject?, byte>> SomeObjectBytePairArray { get; set; } = new();
    public List<UObject?> CrossLevelActors { get; set; } = new();
    public FPrecomputedLightVolume FPrecomputedLightVolume { get; set; } = new();
    public FPrecomputedVisibilityHandler PrecomputedLightVolume { get; set; } = new();
    public FPrecomputedVolumeDistanceField PrecomputedVolumeDistanceField { get; set; } = new();
}

public class FPrecomputedVolumeDistanceField
{
    public float VolumeMaxDistance { get; set; }
    public FBox VolumeBox { get; set; } = new();
    public int VolumeSizeX { get; set; }
    public int VolumeSizeY { get; set; }
    public int VolumeSizeZ { get; set; }
    public List<FColor> Colors { get; set; } = new();
}

public class FPrecomputedVisibilityHandler
{
    public FVector2D PrecomputedVisibilityCellBucketOriginXY { get; set; } = new();
    public float PrecomputedVisibilityCellSizeXY { get; set; }
    public float PrecomputedVisibilityCellSizeZ { get; set; }
    public int PrecomputedVisibilityCellBucketSizeXY { get; set; }
    public int PrecomputedVisibilityNumCellBuckets { get; set; }
    public List<FPrecomputedVisibilityBucket> PrecomputedVisibilityCellBuckets { get; set; } = new();
}

public class FPrecomputedVisibilityBucket
{
    //TODO Fill out once encountered once..
}

public class FPrecomputedLightVolume
{
    public int Initialized { get; set; }
    public FBox Bounds { get; set; } = new();
    public List<FVolumeLightingSample> samples { get; set; } = new();
    public float SampleSpacing { get; set; }
}

public class FVolumeLightingSample
{
    public FVector Position { get; set; } = new();
    public float Radius { get; set; }
    public byte IndirectDirectionTheta { get; set; }
    public byte IndirectDirectionPhi { get; set; }
    public byte EnvironmentDirectionTheta { get; set; }
    public byte EnvironmentDirectionPhi { get; set; }
    public FColor IndirectRadiance { get; set; } = new();
    public FColor EnvironmentRadiance { get; set; } = new();
    public FColor AmbientRadiance { get; set; } = new();
    public byte bShadowedFromDominantLights { get; set; }
}

public class FKCachedPerTriData
{
    public TArray<byte> CachedPerTriData { get; set; } = new();
}

public class FCachedPerTriPhysSMData
{
    public FVector Scale3d { get; set; } = new();
    public int CachedDataIndex { get; set; }
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