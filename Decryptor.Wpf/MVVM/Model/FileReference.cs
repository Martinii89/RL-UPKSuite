using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;

namespace RlUpk.Decryptor.Wpf.MVVM.Model;

/// <summary>
/// Model used for the main window holding a reference to a file we want to unpack and the unpack result
/// </summary>
public partial class FileReference: ObservableObject
{
    public string FilePath { get; init; } = string.Empty;
    public string FileName => Path.GetFileNameWithoutExtension(FilePath);

    [ObservableProperty]
    private string _unpackResult = string.Empty;
}