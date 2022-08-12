using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "Polys", typeof(UObject))]
public class UPolys : UObject
{
    public UPolys(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public TransientArray<FPoly> Element { get; set; } = new();
}

public class FPoly
{
    public UObject? Actor { get; set; }
    public FVector Base { get; set; } = new();
    public int iBrushPoly { get; set; }
    public int iLink { get; set; }
    public string ItemName { get; set; }
    public int LightingChannels { get; set; }
    public FLightmassPrimitiveSettings LightmassSettings { get; set; } = new();
    public UMaterialInterface? Material { get; set; }
    public FVector Normal { get; set; } = new();
    public uint PolyFlags { get; set; }
    public string RulesetVariation { get; set; }
    public float ShadowMapScale { get; set; }
    public int SmoothingMask { get; set; }
    public FVector TextureU { get; set; } = new();
    public FVector TextureV { get; set; } = new();
    public List<FVector> Vertices { get; set; } = new();
}