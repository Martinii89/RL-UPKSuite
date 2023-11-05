using Core.Classes.Core.Properties;
using Core.Classes.Engine;
using Core.Flags;
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
    ///     Removes the script property for the blend mode. This should make it default to opaque.
    ///     We do this to for the randomly colored diffuse material
    /// </summary>
    /// <param name="material"></param>
    public static void RemoveBlendMode(UMaterial material)
    {
        material.ScriptProperties.RemoveAll(x => x.Name == "BlendMode");
    }

    /// <summary>
    ///     Removes the script property for the lighting model. This should make it default to phong.
    ///     We do this to for the randomly colored diffuse material
    /// </summary>
    /// <param name="material"></param>
    public static void RemoveLightingModel(UMaterial material)
    {
        material.ScriptProperties.RemoveAll(x => x.Name == "LightingModel");
    }

    /// <summary>
    ///     Tries to fix issues related to the cooking processing that seems to be removing lots of nodes. Mainly trying to
    ///     just make parameters
    ///     show up in the editor for MICs. Also tries to connect something up to the color input of the materials to make them
    ///     not all be completely black
    /// </summary>
    /// <param name="materialExports"></param>
    public void AddDummyNodesToMaterials(List<UMaterial> materialExports)
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
                var matExpressions = material.ScriptProperties.Find(x => x.Name == "Expressions");
                if (matExpressions is not null)
                {
                    matExpressions.Size += 4;
                    (matExpressions.Value as List<object?>)?.Add(customNode);
                }
            }

            ConnectRandomColorToDiffuseColorProperty(material);
            SpreadOutExpressions(material.Expressions);
            RemoveBlendMode(material);
            RemoveLightingModel(material);
        }
    }

    /// <summary>
    ///     Positions the materialExpressions in a grid in the material editor
    /// </summary>
    /// <param name="expressions"></param>
    public void SpreadOutExpressions(List<UMaterialExpression> expressions)
    {
        if (expressions.Count == 0)
        {
            return;
        }

        var spreadAmount = 150;
        var columns = 6;
        var startingX = 300;
        var x = startingX;
        var y = 0;
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
                x = startingX;
                y += spreadAmount;
            }
        }
    }

    /// <summary>
    ///     Create and link a vector parameter with random RGB values to the material's diffuse property
    /// </summary>
    /// <param name="material"></param>
    public void ConnectRandomColorToDiffuseColorProperty(UMaterial material)
    {
        _packageExporter.GetOrAddName("DiffuseColor");
        _packageExporter.GetOrAddName("LinearColor");
        _packageExporter.GetOrAddName("Expression");
        _packageExporter.GetOrAddName("ScalarMaterialInput");

        ArgumentNullException.ThrowIfNull(material.Class);
        if (material.Class.GetProperty("DiffuseColor") is not UStructProperty diffuseColorProperty)
        {
            return;
        }

        var vectorNodeClass = _packageExporter.AddClassImport("Engine", "MaterialExpressionVectorParameter");
        vectorNodeClass.Deserialize();
        vectorNodeClass.InitProperties();
        var vectorNodeName = _packageExporter.GetOrAddName("EditorColorVector");

        var vectorNode = new UMaterialExpression(vectorNodeName, vectorNodeClass, material, _packageExporter.Package);
        _packageExporter.AddExport(vectorNode);
        material.Expressions.Add(vectorNode);

        ArgumentNullException.ThrowIfNull(vectorNode.Class);
        _packageExporter.GetOrAddName("DefaultValue");
        var defaultValue = vectorNode.Class.GetProperty("DefaultValue") as UStructProperty;
        ArgumentNullException.ThrowIfNull(defaultValue?.Struct);
        defaultValue.Deserialize();

        var random = new Random();
        var defaultValueObject = new Dictionary<string, object>
        {
            ["R"] = random.NextSingle(),
            ["G"] = random.NextSingle(),
            ["B"] = random.NextSingle(),
            ["A"] = 1f
        };

        _packageExporter.GetOrAddName("ParameterName");
        var paramName = vectorNode.Class.GetProperty("ParameterName");
        ArgumentNullException.ThrowIfNull(paramName);
        paramName.Deserialize();
        _packageExporter.GetOrAddName("EditorColor");

        var defaultValueIsImmutable = defaultValue.Struct.HasFlag(StructFlag.Immutable);
        // Temporarily remove the immutable flag so that we can create a FProperty from it
        defaultValue.Struct.StructFlags &= ~(int) StructFlag.Immutable;
        vectorNode.ScriptProperties.Add(defaultValue.CreateFProperty(defaultValueObject));
        vectorNode.ScriptProperties.Add(paramName.CreateFProperty("EditorColor"));
        // Set immutable flag back if needed
        if (defaultValueIsImmutable)
        {
            defaultValue.Struct.StructFlags |= (int) StructFlag.Immutable;
        }

        _packageExporter.GetOrAddName("OutputIndex");
        var valueObject = new Dictionary<string, object>
        {
            ["Expression"] = vectorNode,
            ["OutputIndex"] = 0
        };

        diffuseColorProperty.Deserialize();
        var fproperty = diffuseColorProperty.CreateFProperty(valueObject);
        var matDiffuseColorIndex = material.ScriptProperties.FindIndex(x => x.Name == "DiffuseColor");

        if (matDiffuseColorIndex != -1)
        {
            material.ScriptProperties[matDiffuseColorIndex] = fproperty;
        }
        else
        {
            material.ScriptProperties.Add(fproperty);
        }

        var matExpressions = material.ScriptProperties.Find(x => x.Name == "Expressions");
        if (matExpressions is not null)
        {
            matExpressions.Size += 4;
            (matExpressions.Value as List<object?>)?.Add(vectorNode);
        }
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
        _packageExporter.GetOrAddName("ExpressionInput");
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