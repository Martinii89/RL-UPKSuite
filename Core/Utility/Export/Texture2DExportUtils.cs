using Core.Classes.Engine;

namespace Core.Utility.Export;

public class Texture2DExportUtils
{
    /// <summary>
    ///     Removes the mips with a texture resolution larger than the given limit
    /// </summary>
    /// <param name="textures"></param>
    /// <param name="limit"></param>
    public static void LimitTextureResolution(List<UTexture2D> textures, int limit)
    {
        foreach (var texture in textures)
        {
            if (!texture.FullyDeserialized)
            {
                texture.Deserialize();
            }

            texture.Mips.RemoveAll(x => x.SizeX > limit || x.SizeY > limit);
        }
    }
}