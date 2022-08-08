using Core.Classes.Core.Properties;
using Core.Classes.Engine;
using Core.Types;

namespace Core.Utility.Export;

/// <summary>
///     Utility class for modifying and fixing up cooked materials before export
/// </summary>
public class MaterialExportUtils
{
    private readonly PackageExporter _packageExporter;

    /// <summary>
    ///     Constructs the PackageExporter. It requires the PackageExporter for the functionality to add new names, classes and
    ///     objects
    /// </summary>
    /// <param name="packageExporter"></param>
    public MaterialExportUtils(PackageExporter packageExporter)
    {
        _packageExporter = packageExporter;
    }

    /// <summary>
    ///     Removes the script property for the lighting model if it is MLM_Custom. We do this because UDK knows nothing about
    ///     this lighting model.This should make it default to Phong
    /// </summary>
    /// <param name="material"></param>
    public void RemoveCustomLightingModel(UMaterial material)
    {
        material.ScriptProperties.RemoveAll(x => x.Name == "LightingModel" && x.Value as string == "MLM_Custom");
    }

    /// <summary>
    ///     Tries to fix issues related to the cooking processing that seems to be removing lots of nodes. Mainly trying to
    ///     just make parameters
    ///     show up in the editor for MICs. Also tries to connect something up to the color input of the materials to make them
    ///     not all be completely black
    /// </summary>
    /// <param name="materialExports"></param>
    public void AddDumyNodesToMaterials(List<UMaterial> materialExports)
    {
        var customNodeClass = _packageExporter.AddClassImport("Engine", "MaterialExpressionCustom");
        customNodeClass.Deserialize();
        customNodeClass.InitProperties();
        var customName = _packageExporter.GetOrAddName("WizardNode");
        foreach (var material in materialExports)
        {
            material.Deserialize();
            var materialParameterExpressions = material.GetMaterialParams();
            if (materialParameterExpressions.Count > 0)
            {
                var customNode = new UMaterialExpression(customName, customNodeClass, material, _packageExporter.Package);
                customName = new FName(customName.NameIndex, customName.InstanceNumber + 1);
                _packageExporter.AddExport(customNode);
                material.Expressions.Add(customNode);

                ConnectToSubsurfaceScatteringRadiusProperty(material, customNode);
                ConnectMaterialParamsToCustomNode(materialParameterExpressions, customNode);
                ConnectDiffuseParamToDiffuseColor(material);
                var matExpressions = material.ScriptProperties.Find(x => x.Name == "Expressions");
                if (matExpressions is not null)
                {
                    matExpressions.Size += 4;
                    (matExpressions.Value as List<object?>)?.Add(customNode);
                }
            }

            SpreadOutExpressions(material.Expressions);
            RemoveCustomLightingModel(material);
        }
    }

    /// <summary>
    ///     Searches for a parameter with diffuse in its name. If found it will connect this to the DiffuseColor input of the
    ///     material
    /// </summary>
    /// <param name="material"></param>
    public void ConnectDiffuseParamToDiffuseColor(UMaterial material)
    {
        _packageExporter.GetOrAddName("DiffuseColor");
        _packageExporter.GetOrAddName("Expression");
        var diffuseExpression = FindDiffuseParameterExpression(material);
        if (diffuseExpression is null)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(material.Class);
        if (material.Class.GetProperty("DiffuseColor") is not UStructProperty diffuseColorProperty)
        {
            return;
        }

        var valueObject = new Dictionary<string, object>
        {
            ["Expression"] = diffuseExpression,
            ["OutputIndex"] = 0
        };
        diffuseColorProperty.Deserialize();
        var fproperty = diffuseColorProperty.CreateFProperty(valueObject);
        material.ScriptProperties.Add(fproperty);
    }

    public void SpreadOutExpressions(List<UMaterialExpression> expressions)
    {
        if (expressions.Count == 0)
        {
            return;
        }

        var spreadAmount = 150;
        var columns = 6;
        var x = spreadAmount;
        var y = spreadAmount;
        _packageExporter.GetOrAddName("MaterialExpressionEditorX");
        _packageExporter.GetOrAddName("MaterialExpressionEditorY");
        var editorXProperty = expressions.First().Class?.GetProperty("MaterialExpressionEditorX");
        var editorYProperty = expressions.First().Class?.GetProperty("MaterialExpressionEditorY");
        ArgumentNullException.ThrowIfNull(editorXProperty);
        ArgumentNullException.ThrowIfNull(editorYProperty);

        foreach (var expression in expressions)
        {
            expression.Deserialize();
            expression.ScriptProperties.Add(editorXProperty.CreateFProperty(x));
            expression.ScriptProperties.Add(editorYProperty.CreateFProperty(y));
            x += spreadAmount;
            if (x > columns * spreadAmount)
            {
                x = spreadAmount;
                y += spreadAmount;
            }
        }
    }

    private static UMaterialExpression? FindDiffuseParameterExpression(UMaterial material)
    {
        var paramList = material.GetMaterialParams();
        UMaterialExpression? diffuseExpression = null;
        foreach (var parameterExpression in paramList)
        {
            parameterExpression.Deserialize();
            var paramName = parameterExpression.ScriptProperties.FirstOrDefault(x => x.Name == "ParameterName")?.Value as string;
            if (paramName?.Contains("diffuse", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                diffuseExpression = parameterExpression;
                break;
            }
        }

        return diffuseExpression;
    }

    /// <summary>
    ///     Set up a connection between a material expression and the SubsurfaceScatteringRadius input of the material
    /// </summary>
    /// <param name="material"></param>
    /// <param name="expression"></param>
    public void ConnectToSubsurfaceScatteringRadiusProperty(UMaterial material, UMaterialExpression expression)
    {
        _packageExporter.GetOrAddName("SubsurfaceScatteringRadius");
        _packageExporter.GetOrAddName("Expression");
        var valueObject = new Dictionary<string, object>
        {
            ["Expression"] = expression
        };
        ArgumentNullException.ThrowIfNull(material.Class);
        var rootProperty = material.Class.GetProperty("SubsurfaceScatteringRadius") as UStructProperty;
        ArgumentNullException.ThrowIfNull(rootProperty);
        rootProperty.Deserialize();
        var fproperty = rootProperty.CreateFProperty(valueObject);
        material.ScriptProperties.Add(fproperty);
    }


    /// <summary>
    ///     Connects the given parameters to the input of the given MaterialExpressionCustom node
    /// </summary>
    /// <param name="materialParameterExpressions">A list of parameters to connect</param>
    /// <param name="customNode">Should be of type MaterialExpressionCustom</param>
    public void ConnectMaterialParamsToCustomNode(List<UMaterialExpression> materialParameterExpressions, UMaterialExpression customNode)
    {
        _packageExporter.GetOrAddName("Inputs");
        _packageExporter.GetOrAddName("Input");
        var inputsParams = customNode.Class?.GetProperty("Inputs");
        ArgumentNullException.ThrowIfNull(inputsParams);
        inputsParams.Deserialize();
        var valueObject = new List<object>();
        foreach (var materialParam in materialParameterExpressions)
        {
            var materialInput = new Dictionary<string, object>
            {
                ["Expression"] = materialParam
            };
            var input = new Dictionary<string, object>
            {
                ["Input"] = materialInput
            };
            valueObject.Add(input);
        }

        customNode.ScriptProperties.Add(inputsParams.CreateFProperty(valueObject));
    }
}