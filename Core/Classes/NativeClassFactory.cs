using System.Reflection;
using Core.Classes;
using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Types;

namespace Core;

public class NativeClassFactory : INativeClassFactory
{
    private readonly Dictionary<string, NativeClassInfo> _nativeClasses;

    public NativeClassFactory()
    {
        _nativeClasses = FindNativeClassInfosInExecutingAssembly();
        CreateStubClasses();
    }
    
    /// <summary>
    /// Class instance used as the base for all UClass instances
    /// </summary>
    public UClass? StaticClass { get; private set; }


    public Dictionary<string, UClass> GetNativeClasses(UnrealPackage package)
    {
        Dictionary<string, UClass> result = new();

        var packageNatives = _nativeClasses.Where(x => x.Value.PackageName == package.PackageName);
        foreach (var (_, value) in packageNatives)
        {
            if (value.RegisteredClass is null)
            {
                continue;
            }

            value.RegisteredClass.MoveStubClassToOwnerPackage(package);
            value.RegisteredClass.InstanceSerializer = package.ObjectSerializerFactory?.GetSerializer(value.AssemblyTypeImplementation);
            result.Add(value.ClassName, value.RegisteredClass);
        }


        return result;
    }

    private void CreateStubClasses()
    {
        UnrealPackage stubPackage = new()
        {
            PackageName = "Natives"
        };
        // create stub classes


        foreach (var (key, value) in _nativeClasses)
        {
            var name = stubPackage.GetOrAddName(value.ClassName);
            var clz = new UClass(name, StaticClass, null, stubPackage);
            clz.InstanceConstructor = (objName, outer, package, objArchetype) =>
                (UObject) Activator.CreateInstance(value.AssemblyTypeImplementation, objName, clz, outer, package, objArchetype);
            value.RegisteredClass = clz;
            if (value.ClassName == "Class")
            {
                StaticClass = clz;
            }

            var typePropertiesWithNativeAttribute = value.AssemblyTypeImplementation.GetProperties()
                .Select(x => x.GetCustomAttribute<NativeProperty>()).Where(x => x is not null);
            foreach (var nativeOnlyClassAttribute in typePropertiesWithNativeAttribute)
            {
                if (nativeOnlyClassAttribute is not null)
                {
                    value.RegisteredClass.NativeProperties.Add(nativeOnlyClassAttribute);
                }
            }
        }

        // set the super classes
        foreach (var (key, value) in _nativeClasses)
        {
            if (value.RegisteredClass is null)
            {
                continue;
            }

            value.RegisteredClass.Class = StaticClass;

            var superType = value.SuperType;
            if (superType is null)
            {
                continue;
            }

            var superNative = _nativeClasses.FirstOrDefault(x => x.Value.AssemblyTypeImplementation == superType);

            value.RegisteredClass.SuperClass = superNative.Value.RegisteredClass;
        }
    }

    private static Dictionary<string, NativeClassInfo> FindNativeClassInfosInExecutingAssembly()
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

    private class NativeClassInfo(Type assemblyTypeImplementation, NativeOnlyClassAttribute attribute)
    {
        internal string ClassFullName => $"{PackageName}.{ClassName}";
        internal string ClassName { get; set; } = attribute.ClassName;
        internal string PackageName { get; set; } = attribute.PackageName;
        internal string SuperClassName { get; set; } = attribute.SuperClass;

        internal Type? SuperType { get; set; } = attribute.SuperType;

        internal UClass? RegisteredClass { get; set; } = null;
        internal Type AssemblyTypeImplementation { get; set; } = assemblyTypeImplementation;
    }
}