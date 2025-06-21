using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using SharpGLTF.Validation;

namespace RlUpk.MapBuilder.Cli;

public class ModelFinder(string rootFolder)
{
    private static readonly ReadSettings ReadSettings = new() { Validation = ValidationMode.Skip };

    public SceneBuilder? LoadSceneBuilder(string fullName)
    {
        try
        {
            var parts = fullName.Split('.');
            var path = Path.Combine([rootFolder, ..parts[1..^1], $"{parts[^1]}.gltf"]);
            return File.Exists(path) ? SceneBuilder.LoadDefaultScene(path, ReadSettings) : null;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error while loading: {fullName}");
            Console.Error.WriteLine(e);
            return null;
        }
    }
}