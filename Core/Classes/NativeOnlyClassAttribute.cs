namespace Core.Classes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class NativeOnlyClassAttribute : Attribute
{
    public NativeOnlyClassAttribute(string packageName, string className, string superClass = "")
    {
        ClassName = className;
        SuperClass = superClass;
        PackageName = packageName;
    }

    /// <summary>
    ///     The name other objects should use to find this class
    /// </summary>
    public string ClassName { get; }

    /// <summary>
    ///     The name of the super class. May be empty
    /// </summary>
    public string SuperClass { get; }

    /// <summary>
    ///     The outer for this class. The package where it should be injected
    /// </summary>
    public string PackageName { get; }
}