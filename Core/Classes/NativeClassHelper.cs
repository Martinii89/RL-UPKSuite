﻿using System.Reflection;
using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

/// <summary>
///     This class is used by the unreal package to find and initialize the native only classes defined in the assembly
/// </summary>
public class NativeClassRegistrationHelper
{
    /// <summary>
    ///     Constructs a NativeClassRegistrationHelper with a given outer that all the registered native classes should use as
    ///     their outer
    /// </summary>
    /// <param name="outerPackage"></param>
    public NativeClassRegistrationHelper(UPackage outerPackage)
    {
        OuterPackage = outerPackage;
    }

    /// <summary>
    ///     The outer all the native classes should use
    /// </summary>
    public UPackage OuterPackage { get; }

    private List<NativeClassRegistration> GetNativeClassesFromExecutingAssembly()
    {
        return Assembly.GetExecutingAssembly().GetExportedTypes()
            .Select(t => new { t, attribute = t.GetCustomAttribute<NativeOnlyClassAttribute>(false) })
            .Where(x => x.attribute != null)
            .Select(x => new NativeClassRegistration(x.t, x.attribute!))
            .ToList();
    }

    private Dictionary<string, List<NativeClassRegistration>> GroupNativeBySuperClass(IList<NativeClassRegistration> nativeClassRegistrations)
    {
        return nativeClassRegistrations.GroupBy(x => x.NativeOnlyClassAttribute.SuperClass).ToDictionary(y => y.Key, y => y.ToList());
    }

    /// <summary>
    ///     Constructs and returns the native classes in the executing assembly. They are registered to the corePackage and
    ///     will have their owner package set to this
    /// </summary>
    /// <param name="corePackage"></param>
    /// <returns></returns>
    public Dictionary<string, UClass> GetNativeClasses(UnrealPackage corePackage)
    {
        var nativeClassesFromExecutingAssembly = GetNativeClassesFromExecutingAssembly();
        var groupNativeBySuperClass = GroupNativeBySuperClass(nativeClassesFromExecutingAssembly);

        var registeredClasses = new Dictionary<string, UClass>();

        // Start with those requiring no specific base class
        var registerQueue = new Queue<NativeClassRegistration>(groupNativeBySuperClass[""]);

        while (registerQueue.Count > 0)
        {
            var typeToRegister = registerQueue.Dequeue();
            var attribute = typeToRegister.NativeOnlyClassAttribute;

            var className = attribute.ClassName;
            var superClass = attribute.SuperClass == string.Empty ? null : registeredClasses[attribute.SuperClass];
            var classFName = corePackage.GetOrAddName(className);
            var newClass = new UClass(classFName, UClass.StaticClass, OuterPackage, corePackage, superClass);
            registeredClasses[attribute.ClassName] = newClass;
            if (groupNativeBySuperClass.TryGetValue(className, out var derivedList))
            {
                derivedList.ForEach(registerQueue.Enqueue);
            }
        }

        var classClass = registeredClasses["Class"];
        foreach (var (name, registeredClass) in registeredClasses)
        {
            registeredClass.Class = classClass;
        }

        OuterPackage.Class = registeredClasses["Package"];

        return registeredClasses;
    }


    private class NativeClassRegistration
    {
        public NativeClassRegistration(Type type, NativeOnlyClassAttribute nativeOnlyClassAttribute)
        {
            Type = type;
            NativeOnlyClassAttribute = nativeOnlyClassAttribute;
        }

        private Type Type { get; }
        public NativeOnlyClassAttribute NativeOnlyClassAttribute { get; }
    }
}