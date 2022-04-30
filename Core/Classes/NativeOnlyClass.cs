namespace Core.Classes;

public class NativeOnlyClassAttribute : Attribute
{
    public NativeOnlyClassAttribute(string packageName, string className)
    {
        ClassName = className;
        PackageName = packageName;
    }

    /// <summary>
    ///     The name other objects should use to find this class
    /// </summary>
    public string ClassName { get; }

    /// <summary>
    ///     The outer for this class. The package where it should be injected
    /// </summary>
    public string PackageName { get; }
}