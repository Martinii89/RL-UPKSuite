using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "Material", typeof(UMaterialInterface))]
public class UMaterial : UMaterialInterface
{
    public UMaterial(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public List<UMaterialExpression> Expressions { get; set; } = new();

    public List<FMaterialResource> FMaterialResources { get; set; } = new();
    public int ResourceCountFlag { get; set; }

    /// <summary>
    ///     Find the Expressions FProperty in the ScriptProperties and adds all the MaterialExpressions in there to the
    ///     expressions list for easy access
    /// </summary>
    public void InitMateralExpressionsList()
    {
        if (Expressions.Count > 0)
        {
            return;
        }

        var expressionsProperty = ScriptProperties.Find(x => x.Name == "Expressions");

        if (expressionsProperty?.Value is not List<object> expressionsList)
        {
            return;
        }

        foreach (var o in expressionsList)
        {
            if (o is UMaterialExpression expr)
            {
                Expressions.Add(expr);
            }
        }
    }

    /// <summary>
    ///     Returns a list of all the material expressions that are material parameters. The material has to be deserialized
    ///     first
    /// </summary>
    /// <returns></returns>
    public List<UMaterialExpression> GetMaterialParams()
    {
        InitMateralExpressionsList();
        return Expressions.Where(x =>
            x.Class != null && (x.Class.IsA("MaterialExpressionParameter") || x.Class.IsA("MaterialExpressionTextureSampleParameter"))).ToList();
    }
}

public class FMaterial
{
    public List<string> CompileErrors { get; set; } = new();
    public Dictionary<UMaterialExpression, int> TextureDependencyLengthMap { get; set; } = new();
    public int MaxTextureDependencyLength { get; set; }
    public FGuid ID { get; set; }
    public uint NumUserTexCoords { get; set; }
    public List<UTexture?> UniformExpressionTextures { get; set; } = new();
    public bool bUsesSceneColorTemp { get; set; }
    public bool bUsesSceneDepthTemp { get; set; }
    public bool bUsesDynamicParameterTemp { get; set; }
    public bool bUsesLightmapUVsTemp { get; set; }
    public bool bUsesMaterialVertexPositionOffsetTemp { get; set; }
    public bool UsingTransforms { get; set; }
    public List<FTextureLookupInfo> FTextureLookupInfos { get; set; } = new();
    public int Unk { get; set; }
}

public class FMaterialResource : FMaterial
{
    public int Unk1 { get; set; }
    public int Unk2 { get; set; }
    public int Unk3 { get; set; }
}

public class FTextureLookupInfo
{
    public int TexCoordIndex { get; set; }
    public int TextureIndex { get; set; }
    public float UScale { get; set; }
    public float VScale { get; set; }
}