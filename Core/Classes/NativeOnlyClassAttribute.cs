namespace Core.Classes;

/// <summary>
///     Marks a class for injection into it's owning package as a native only class
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class NativeOnlyClassAttribute : Attribute
{
    /// <summary>
    ///     Defined the name of the package, class and super class for this native only class
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="className"></param>
    /// <param name="superType"></param>
    public NativeOnlyClassAttribute(string packageName, string className, Type? superType = null)
    {
        SuperType = superType;
        ClassName = className;
        PackageName = packageName;
    }

    /// <summary>
    ///     The SuperType
    /// </summary>
    public Type? SuperType { get; }

    /// <summary>
    ///     The name other objects should use to find this class
    /// </summary>
    public string ClassName { get; }

    /// <summary>
    ///     The name of the super class. May be empty
    /// </summary>
    public string SuperClass => SuperType == null ? string.Empty : SuperType.Name[1..];


    /// <summary>
    ///     The outer for this class. The package where it should be injected
    /// </summary>
    public string PackageName { get; }
}