using System.IO;

namespace Decryptor.Wpf.MVVM.Model;

public class FileReference
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName => Path.GetFileNameWithoutExtension(FilePath);
}