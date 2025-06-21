using CommandLine;

namespace RlUpk.MapBuilder.Cli;

public class MapBuilderOptions
{
    [Option("import_folder", Required = false,
        HelpText =
            "Path to the folder containing the dependant import packages (core,engine, etc). The program will use the folder of the first package to process if this value is not provided")]
    public string ImportPackagesDirectory { get; set; } = string.Empty;
    
    [Option("assets", Required = false, HelpText = "Path to where exported meshes are stored before re-assembly")]
    public string AssetsFolder { get; set; } = "./assets/";
    
    [Option("decrypted-folder", Required = false, HelpText = "Folder to store decrypted packages that UModel needs to rip the assets")]
    public string DecryptedFolder { get; set; } = "./decrypted/";
    

    [Option('f', "files", Required = false, HelpText = "List of files")]
    public IEnumerable<string>? Files { get; set; }

    [Option('k', "keyfile", Default = "keys.txt", Required = false, HelpText = "Path to file with decryption keys")]
    public string KeysPath { get; set; } = string.Empty;
    
    [Option('g', "glob", Required = false, HelpText = "Glob patterns for files to process")]
    public IEnumerable<string>? GlobPatterns { get; set; }


    public string GetCommandLineString()
    {
        return Parser.Default.FormatCommandLine(this);
    }
}