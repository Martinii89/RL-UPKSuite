using CommandLine;

namespace RlUpk.RLToUdkConverter;

public class PackageConversionOptions
{
    //[Option('f', "folder", Required = true, HelpText = "Path to the folder containing the packages")]
    //public string InputDirectory { get; set; } = string.Empty;
    //[Option('g', "glob", Required = false, HelpText = "Glob pattern for filtering packages")]
    //public string GlobPattern { get; set; } = string.Empty;

    [Option("import_folder", Required = false,
        HelpText =
            "Path to the folder containing the dependant import packages (core,engine, etc). The program will use the folder of the first package to process if this value is not provided")]
    public string ImportPackagesDirectory { get; set; } = string.Empty;

    [Option('o', "out", Required = true, HelpText = "Path to the folder where to place the exported packages")]
    public string OutputDirectory { get; set; } = string.Empty;

    [Option('f', "files", Required = false, HelpText = "List of files")]
    public IEnumerable<string>? Files { get; set; }

    [Option('k', "keyfile", Default = "keys.txt", Required = false, HelpText = "Path to file with decryption keys")]
    public string KeysPath { get; set; } = string.Empty;

    [Option("suffix", Required = false, HelpText = "Suffix to append to converted file")]
    public string Suffix { get; set; } = string.Empty;

    [Option('c', "compress", Required = false, HelpText = "Toggle for adding compression to the converted packages")]
    public bool Compress { get; set; } = false;
    
    [Option('g', "glob", Required = false, HelpText = "Glob patterns for files to process")]
    public IEnumerable<string>? GlobPatterns { get; set; }

    [Option('t', "threads", Required = false, HelpText = "How many parallel threads to use")]
    public int Threads { get; set; } = 1;


    public string GetCommandLineString()
    {
        return Parser.Default.FormatCommandLine(this);
    }
}