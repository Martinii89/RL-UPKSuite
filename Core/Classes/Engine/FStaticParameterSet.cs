using Core.Types;

namespace Core.Classes.Engine;

public class FStaticParameterSet
{
    public FGuid BaseMaterialID { get; set; } = new();
    public List<FStaticSwitchParameter> StaticSwitchParameters { get; set; } = new();
    public List<FStaticComponentMaskParameter> StaticComponentMaskParameters { get; set; } = new();
    public List<FNormalParameter> NormalParameters { get; set; } = new();
    public List<FStaticTerrainLayerWeightParameter> TerrainLayerWeightParameters { get; set; } = new();
}

public class FStaticTerrainLayerWeightParameter
{
    public FName ParameterName { get; set; }
    public bool bOverride { get; set; }
    public FGuid ExpressionGUID { get; set; } = new();
    public int WeightmapIndex { get; set; }
}

public class FNormalParameter
{
    public FName ParameterName { get; set; }
    public byte CompressionSettings { get; set; }
    public bool bOverride { get; set; }
    public FGuid ExpressionGUID { get; set; } = new();
}

public class FStaticComponentMaskParameter
{
    public FName ParameterName { get; set; }
    public bool R { get; set; }
    public bool G { get; set; }
    public bool B { get; set; }
    public bool A { get; set; }
    public bool bOverride { get; set; }
    public FGuid ExpressionGUID { get; set; } = new();
}

public class FStaticSwitchParameter
{
    public FName ParameterName { get; set; }
    public bool Value { get; set; }
    public bool bOverride { get; set; }
    public FGuid ExpressionGUID { get; set; } = new();
}