using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

public class USurface : UObject
{
    public USurface(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}

public class UMaterialInterface : USurface
{
    public UMaterialInterface(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer, ownerPackage, objectArchetype)
    {
    }
}

public class UMaterial : UMaterialInterface
{
    public UMaterial(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public FMaterialResource[] FMaterialResources { get; set; } = new FMaterialResource[2];
}

public class UMaterialExpression : UObject
{
    public UMaterialExpression(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer, ownerPackage, objectArchetype)
    {
    }
}

public class UTexture : USurface
{
    public UTexture(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}

public class FMaterial
{
    // 16 unknown bytes at the end 
    internal byte[]? UnknownBytes;
    public List<string> CompileErrors { get; set; } = new();
    public Dictionary<UMaterialExpression, int> TextureDependencyLengthMap { get; set; } = new();

    public int MaxTextureDependencyLength { get; set; }

    public FGuid ID { get; set; }

    public uint NumUserTexCoords { get; set; }

    public List<UTexture> UniformExpressionTextures { get; set; }

    public bool bUsesSceneColorTemp { get; set; }
    public bool bUsesSceneDepthTemp { get; set; }
    public bool bUsesDynamicParameterTemp { get; set; }
    public bool bUsesLightmapUVsTemp { get; set; }
    public bool bUsesMaterialVertexPositionOffsetTemp { get; set; }
    public bool UsingTransforms { get; set; }
    public FTextureLookupInfo FTextureLookupInfos { get; set; }
    public bool DummyDroppedFallbackComponents { get; set; }
}

public class FMaterialResource : FMaterial
{
}

public class FTextureLookupInfo
{
    public int TexCoordIndex { get; set; }
    public int TextureIndex { get; set; }
    public float UScale { get; set; }
    public float VScale { get; set; }
}