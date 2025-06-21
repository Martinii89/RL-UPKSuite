using SharpGLTF.Scenes;

namespace RlUpk.MapBuilder.Cli;

public class ModelFinder(string rootFolder)
{
    public SceneBuilder? LoadSceneBuilder(string fullName)
    {
        try
        {
            var parts = fullName.Split('.');
            var path = Path.Combine([rootFolder, ..parts[1..^1], $"{parts[^1]}.gltf"]);
            return File.Exists(path) ? SceneBuilder.LoadDefaultScene(path) : null;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            return null;
        }
    }
}