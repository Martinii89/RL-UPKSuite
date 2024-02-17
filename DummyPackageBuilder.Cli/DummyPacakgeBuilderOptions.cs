using CommandLine;

namespace DummyPackageBuilderCli;

public class DummyPacakgeBuilderOptions
{

    [Option('s', "scripts", Required = false,
        HelpText =
            @"Path to the folder containing the udk scripts. Default to C:\UDK\Custom\UDKGame\Script")]
    public string ScriptsDirectory { get; set; } = @"C:\UDK\Custom\UDKGame\Script";

    [Option('o', "out", Required = true, HelpText = "Path to the folder where to place the exported packages")]
    public string OutputDirectory { get; set; } = string.Empty;

    [Option('i', "input", Required = true, HelpText = "List of package definitions")]
    public IEnumerable<string> PackageDefinitionFiles { get; set; }
    


    public string GetCommandLineString()
    {
        return Parser.Default.FormatCommandLine(this);
    }
}