using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Decryptor.Wpf.MVVM.Model;

/// <summary>
/// Enum for the unpack result
/// </summary>
public enum UnpackResult
{
    None = 0,
    Success,
    Fail
}

/// <summary>
/// Model used for the main window holding a reference to a file we want to unpack and the unpack result
/// </summary>
public partial class FileReference: ObservableObject
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName => Path.GetFileNameWithoutExtension(FilePath);

    [ObservableProperty]
    private UnpackResult _unpackResult = UnpackResult.None;
}