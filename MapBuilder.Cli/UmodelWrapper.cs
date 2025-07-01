using CliWrap.Buffered;

namespace RlUpk.MapBuilder.Cli;

public class UmodelWrapper
{
    public static async Task ExportMeshes(string packagePath, string outputPath)
    {
        var result = await CliWrap.Cli.Wrap("umodel.exe")
            .WithArguments(args => args
                .Add("-export")
                .Add("-gltf")
                .Add("-uncook")
                .Add("-groups")
                .Add("-nooverwrite")
                .Add("-nolightmap")
                .Add("-game=rocketleague")
                .Add($"-out=\"{outputPath}\"")
                .Add(packagePath)
            )
            .ExecuteBufferedAsync();
        
        var output = result.StandardOutput;
        var lastLine = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Last();
        if (!lastLine.Contains("Exported"))
        {
            throw new Exception("Failed to export meshes");
        }
    }
}