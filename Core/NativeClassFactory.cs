using System.Reflection;
using Core.Classes;
using Core.Classes.Core;
using Core.Types;

namespace Core;

public class NativeClassFactory : INativeClassFactory
{
    private readonly Dictionary<string, NativeClassInfo> NativeClasses = new();

    public NativeClassFactory()
    {
        FindNativeClassInfosInExecutingAssembly();
    }


    public Dictionary<string, UClass> GetNativeClasses(UnrealPackage package)
    {
        var packageName = package.PackageName;
        Dictionary<string, UClass> result = new();

        foreach (var (key, value) in NativeClasses)
        {
        }

        return result;
    }

    private Dictionary<string, NativeClassInfo> FindNativeClassInfosInExecutingAssembly()
    {
        Dictionary<string, NativeClassInfo> result = new();
        var nativeTypes = Assembly.GetExecutingAssembly().GetExportedTypes()
            .Select(t => new { t, attribute = t.GetCustomAttribute<NativeOnlyClassAttribute>(false) })
            .Where(x => x.attribute != null);
        foreach (var type in nativeTypes)
        {
            ArgumentNullException.ThrowIfNull(type.attribute);
            var info = new NativeClassInfo(type.t, type.attribute);
            result.Add(info.ClassFullName, info);
        }

        return result;
    }

    internal class NativeClassInfo
    {
        public NativeClassInfo(Type assemblyTypeImplementation, NativeOnlyClassAttribute attribute)

        {
            PackageName = attribute.PackageName;
            ClassName = attribute.ClassName;
            AssemblyTypeImplementation = assemblyTypeImplementation;
            RegisteredClass = null;
            SuperClassName = attribute.SuperClass;
        }

        internal string ClassFullName => $"{ClassName}.{PackageName}";
        internal string ClassName { get; set; }
        internal string PackageName { get; set; }
        internal string SuperClassName { get; set; }
        internal UClass? RegisteredClass { get; set; }
        internal Type AssemblyTypeImplementation { get; set; }
    }
}