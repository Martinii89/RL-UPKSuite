namespace Core.Serialization;

[AttributeUsage(AttributeTargets.Class)]
public class FileVersionAttribute : Attribute
{
    public readonly string FileVersion;

    public FileVersionAttribute(string fileVersion)
    {
        FileVersion = fileVersion;
    }
}