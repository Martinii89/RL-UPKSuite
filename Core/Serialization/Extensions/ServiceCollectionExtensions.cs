using System.Reflection;
using Core.Serialization;
using Core.Serialization.Abstraction;
using Microsoft.Extensions.DependencyInjection.Extensions;


// MS recommends placing extensions methods into this namespace: Source https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Options object for SerializerExtensions.UseSerializers where you can defined the serializer.
/// </summary>
public class SerializerOptions
{
    /// <summary>
    ///     Enum for deciding if default serializers should be used in addition to specific version serializers
    /// </summary>
    public enum DefaultSerializers
    {
        /// <summary>
        ///     Use default serializers
        /// </summary>
        Yes,

        /// <summary>
        ///     Don't use default serializers
        /// </summary>
        No
    }

    /// <summary>
    ///     Use all default options.
    /// </summary>
    public SerializerOptions()
    {
    }

    /// <summary>
    ///     Set file version for serializers and optionally use default serializers as well
    /// </summary>
    /// <param name="fileVersion"></param>
    /// <param name="useDefaultSerializers"></param>
    /// <param name="defaultAssembly"></param>
    public SerializerOptions(string fileVersion, DefaultSerializers useDefaultSerializers = DefaultSerializers.Yes, Assembly? defaultAssembly = null)
    {
        FileVersion = fileVersion;
        UseDefaultSerializers = useDefaultSerializers;
        DefaultAssembly = defaultAssembly;
    }

    /// <summary>
    ///     Filter the serializers by this file version
    /// </summary>
    public string FileVersion { get; init; } = string.Empty;

    /// <summary>
    ///     If default serializers should be included for the types where one tagged with FileVersion is found
    /// </summary>
    public DefaultSerializers UseDefaultSerializers { get; set; }

    /// <summary>
    ///     In which assembly to search for default serializers. Default to the same assembly as the FileVersion ones if null.
    /// </summary>
    public Assembly? DefaultAssembly { get; set; }
}

/// <summary>
///     Extension for easilly registering serializers from a specific assembly
/// </summary>
public static class SerializerExtensions
{
    private static readonly Type SerializerInterfaceType = typeof(IStreamSerializer<>);
    private static readonly Type GenericObjectSerializers = typeof(IObjectSerializer<>);
    private static readonly Type ObjectSerializers = typeof(IObjectSerializer);

    public static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        if (assembly == null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t != null)!;
        }
    }

    private static List<Type> GetImplementedSerializersOnType(Type type)
    {
        return type.GetInterfaces()
            .Where(i => i.IsGenericType &&
                        (i.GetGenericTypeDefinition() == SerializerInterfaceType || i.GetGenericTypeDefinition() == GenericObjectSerializers))
            .ToList();
    }

    private static List<Type> GetImplementedGenericSerializersOnType(Type type, Type genericInterface)
    {
        return type.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface)
            .ToList();
    }

    private static bool ImplementeInterface(Type type, Type @interface)
    {
        return type.IsAssignableTo(@interface);
    }

    private static List<AssemblySearchResult> GetSerializersFromAssembly(Assembly assembly, string tag)
    {
        var types = new List<AssemblySearchResult>();
        foreach (var type in GetLoadableTypes(assembly))
        {
            if (type.IsAbstract || type.IsInterface)
            {
                continue;
            }

            var versionTag = type.GetCustomAttribute<FileVersionAttribute>()?.FileVersion ?? string.Empty;
            if (versionTag != tag)
            {
                continue;
            }

            var serializerInterfaces = new List<Type>();
            serializerInterfaces.AddRange(GetImplementedGenericSerializersOnType(type, SerializerInterfaceType));
            serializerInterfaces.AddRange(GetImplementedGenericSerializersOnType(type, GenericObjectSerializers));
            //if (ImplementeInterface(type, ObjectSerializers))
            //{
            //    serializerInterfaces.Add(ObjectSerializers);
            //}

            if (serializerInterfaces.Any())
            {
                types.Add(new AssemblySearchResult(type)
                {
                    Interfaces = serializerInterfaces,
                    VersionInfo = versionTag
                });
            }
        }

        return types;
    }

    private static List<Type> GetImplementedSerializersOnTypeWithBasedRemoved(Type type)
    {
        var possibleInterfaces = GetImplementedSerializersOnType(type);
        if (type.BaseType == null)
        {
            return possibleInterfaces;
        }

        var interfacesInBase = GetImplementedSerializersOnType(type.BaseType);
        interfacesInBase.ForEach(i => possibleInterfaces.Remove(i));

        return possibleInterfaces;
    }

    /// <summary>
    ///     Scans the assembly and adds the found serializer with a matching FileVersion. Replaces previously registered
    ///     serializers
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assembly">The assembly to scan</param>
    /// <param name="options">Configure FileVersion and DefaultSerializer options</param>
    /// <returns></returns>
    public static IServiceCollection AddSerializers(this IServiceCollection services, Assembly assembly, SerializerOptions options)
    {
        var serializersToAdd = GetSerializersFromAssembly(assembly, options.FileVersion);
        Dictionary<Type, Type> interfaceImplementationMap = new();
        foreach (var scanResult in serializersToAdd)
        {
            foreach (var serializerInterface in scanResult.Interfaces)
            {
                interfaceImplementationMap.Add(serializerInterface, scanResult.Type);
            }
        }

        if (options.UseDefaultSerializers == SerializerOptions.DefaultSerializers.Yes && !string.IsNullOrEmpty(options.FileVersion))
        {
            var defaultTypes = GetSerializersFromAssembly(options.DefaultAssembly ?? assembly, "");
            foreach (var scanResult in defaultTypes)
            {
                foreach (var serializerInterface in scanResult.Interfaces.Where(serializerInterface =>
                             !interfaceImplementationMap.ContainsKey(serializerInterface)))
                {
                    interfaceImplementationMap.Add(serializerInterface, scanResult.Type);
                }
            }
        }

        foreach (var (@interface, type) in interfaceImplementationMap)
        {
            AddOrReplaceSingletonService(services, @interface, type);
            if (type.IsAssignableTo(ObjectSerializers))
            {
                AddOrReplaceSingletonService(services, ObjectSerializers, type);
            }
        }

        return services;
    }

    private static void AddOrReplaceSingletonService(IServiceCollection services, Type serializerInterface, Type type)
    {
        var service = new ServiceDescriptor(serializerInterface, type, ServiceLifetime.Singleton);
        if (services.Contains(service))
        {
            services.Replace(service);
        }
        else
        {
            services.Add(service);
        }
    }


    /// <summary>
    ///     Scans the assembly of the type and adds the found serializer with a matching FileVersion. Replaces previously
    ///     registered serializers
    /// </summary>
    /// <param name="services"></param>
    /// <param name="typeInAssembly">A type in the assembly to scan</param>
    /// <param name="options">Configure FileVersion and DefaultSerializer options</param>
    /// <returns></returns>
    public static IServiceCollection AddSerializers(this IServiceCollection services, Type typeInAssembly, SerializerOptions options)
    {
        return services.AddSerializers(typeInAssembly.Assembly, options);
    }

    private class AssemblySearchResult
    {
        public readonly Type Type;
        public List<Type> Interfaces = new();
        public string VersionInfo = string.Empty;

        public AssemblySearchResult(Type type)
        {
            Type = type;
        }
    }
}