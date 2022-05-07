using System.Text;
using Core.Classes;
using Core.Classes.Core;
using Core.Serialization;
using Core.Types.PackageTables;
using Core.Utility;

namespace Core.Types;

/// <summary>
///     A UnrealPackage is the deserialized data from a UPK file. These files can contain all kinds of unreal object for a
///     game or specific maps..
/// </summary>
public class UnrealPackage
{
    private const string CorePackageName = "Core";

    public readonly List<UClass> PackageClasses = new();
    public IImportResolver? ImportResolver { get; set; }

    public PackageLoader? RootLoader { get; set; }

    /// <summary>
    ///     The name of the package. IF this is set to "Core", some additional logic will run to inject the native only
    ///     classes.
    /// </summary>
    public string PackageName { get; set; } = string.Empty;

    /// <summary>
    ///     The root package object. All top level objects will get this as their outer.
    /// </summary>
    public UPackage? PackageRoot { get; set; }


    /// <summary>
    ///     The header summarizes what the package contains and where in the file the different parts are located
    /// </summary>
    public FileSummary Header { get; set; } = new();

    /// <summary>
    ///     The name table contains all the names that this package references
    /// </summary>
    public NameTable NameTable { get; } = new();

    /// <summary>
    ///     The import table references all the objects that this package depends on
    /// </summary>
    public ImportTable ImportTable { get; } = new();

    /// <summary>
    ///     The export table contains all the objects that this package defines.
    /// </summary>
    public ExportTable ExportTable { get; } = new();

    /// <summary>
    ///     The depends table will tell which objects each export depends on. For cooked packages this will be empty
    /// </summary>
    public DependsTable DependsTable { get; } = new();

    /// <summary>
    ///     The Thumbnail table contains data used by the editor to show thumbnails in the asset browser. Whenever a object has
    ///     a custom thumbnail, It will be defined here
    /// </summary>
    public ThumbnailTable ThumbnailTable { get; } = new();

    /// <summary>
    ///     Helper constructor to create and initialize a package from a <see cref="IStreamSerializerFor{T}" />
    /// </summary>
    /// <param name="stream">The package stream</param>
    /// <param name="deserializer">A Serializer compatible with the stream</param>
    /// <param name="packageName">The name of this package.</param>
    /// <param name="importResolver">Used to resolve import objects</param>
    /// <returns></returns>
    public static UnrealPackage DeserializeAndInitialize(Stream stream, IStreamSerializerFor<UnrealPackage> deserializer, string packageName,
        IImportResolver? importResolver = null)
    {
        var package = deserializer.Deserialize(stream);
        package.ImportResolver = importResolver;
        package.PostDeserializeInitialize(packageName);
        return package;
    }

    /// <summary>
    ///     Sets the package name and creates the root package object.
    ///     Additionally: If it the core package native classes will be injected.
    /// </summary>
    /// <param name="packageName"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void PostDeserializeInitialize(string packageName)
    {
        PackageName = packageName;

        PackageRoot = new UPackage(GetOrAddName(packageName), null, null, this);
        if (PackageName == CorePackageName)
        {
            AddCoreNativeClasses();
        }
        else
        {
            AddCoreNativeClassesFromImports();
        }

        var packageClass = FindClass("Package");
        PackageRoot.Class = packageClass;
    }

    private void AddCoreNativeClassesFromImports()
    {
        var nativeObjects = GetNativeObjectsFromImportsInPackage();
        var nativeClasses = nativeObjects.Where(x => GetName(x.ClassName) == "Class").ToList();

        foreach (var nativeClass in nativeClasses)
        {
            nativeObjects.Remove(nativeClass);

            var nativeUClass = new UClass(nativeClass.ObjectName, UClass.StaticClass, PackageRoot, this);
            nativeClass.ImportedObject = nativeUClass;
            PackageClasses.Add(nativeUClass);
        }

        foreach (var nativeObject in nativeObjects)
        {
            var classPackageName = GetName(nativeObject.ClassPackage);
            UnrealPackage? classPackage;
            if (classPackageName != PackageName)
            {
                ArgumentNullException.ThrowIfNull(ImportResolver);
                classPackage = ImportResolver.ResolveExportPackage(classPackageName);
            }
            else
            {
                classPackage = this;
            }

            ArgumentNullException.ThrowIfNull(classPackage);
            var cls = classPackage.FindClass(GetName(nativeObject.ClassName));
            // skip the outers for now. Link them up after creating the objects
            var obj = new UObject(nativeObject.ObjectName, cls, null, this);
            nativeObject.ImportedObject = obj;
        }

        foreach (var nativeObject in nativeObjects)
        {
            if (nativeObject.ImportedObject is not null)
            {
                nativeObject.ImportedObject.Outer = FindOuter(nativeObject);
            }
        }
    }


    /// <summary>
    ///     Returns the ObjectResource of the ObjectIndex. This will either be a <see cref="ImportTableItem" /> from the import
    ///     table or a <see cref="ExportTableItem" /> from the export table
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    internal IObjectResource? GetObjectReference(ObjectIndex index)
    {
        return index.GetReferencedTable() switch
        {
            ObjectIndex.ReferencedTable.Null => null,
            ObjectIndex.ReferencedTable.Import => ImportTable[index.ImportIndex],
            ObjectIndex.ReferencedTable.Export => ExportTable[index.ExportIndex],
            _ => throw new IndexOutOfRangeException(index.GetReferencedTable().ToString())
        };
    }

    /// <summary>
    ///     Returns the name from the name table
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public string GetName(FName name)
    {
        if (name.NameIndex >= NameTable.Count)
        {
            throw new IndexOutOfRangeException($"Invalid FName index {name.NameIndex}");
        }

        return NameTable[name.NameIndex].Name;
    }

    /// <summary>
    ///     Searches the PackageClass cache for a class with this name
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    public UClass? FindClass(string className)
    {
        return PackageClasses.FirstOrDefault(x => x.Name == className);
    }

    /// <summary>
    ///     Constructs the full name of a object. The full name of an object consists of the object name prepended with all the
    ///     outer object separated by a dot
    /// </summary>
    /// <param name="objectResource"></param>
    /// <returns></returns>
    public string GetFullName(IObjectResource objectResource)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(GetName(objectResource.ObjectName));
        var outer = GetObjectReference(objectResource.OuterIndex);

        while (outer != null)
        {
            var outerName = GetName(outer.ObjectName);
            stringBuilder.Insert(0, ".");
            stringBuilder.Insert(0, outerName);
            outer = GetObjectReference(outer.OuterIndex);
        }

        if (objectResource is ExportTableItem)
        {
            stringBuilder.Insert(0, ".");
            stringBuilder.Insert(0, PackageName);
        }


        return stringBuilder.ToString();
    }


    /// <summary>
    ///     Find the package where a import originates from
    /// </summary>
    /// <param name="objectResource"></param>
    /// <returns></returns>
    public IObjectResource GetImportPackage(ImportTableItem objectResource)
    {
        if (GetName(objectResource.ClassName) == "Package" && objectResource.OuterIndex.Index == 0)
        {
            return objectResource;
        }

        var outer = GetObjectReference(objectResource.OuterIndex);
        ArgumentNullException.ThrowIfNull(outer);
        var lastOuter = outer;
        while (outer != null)
        {
            lastOuter = outer;
            outer = GetObjectReference(outer.OuterIndex);
        }

        return lastOuter;
    }


    public UObject? CreateImport(ImportTableItem importTableItem)
    {
        if (ImportResolver == null)
        {
            throw new InvalidDataException("Can't resolve imports without a import resolver");
        }

        if (importTableItem.ImportedObject is not null)
        {
            return importTableItem.ImportedObject;
        }

        var importFullName = GetFullName(importTableItem);
        var importPackageName = importFullName.Split(".")[0];

        //var ClassPackageName = GetName(importTableItem.ClassPackage);
        var importPackage = ImportResolver.ResolveExportPackage(importPackageName);
        if (importPackage == null)
        {
            return null;
        }


        var importOuter = GetObjectReference(importTableItem.OuterIndex);
        if (importOuter is ImportTableItem { ImportedObject: null } import)
        {
            import.ImportedObject = CreateImport(import);
        }

        var className = GetName(importTableItem.ClassName);
        if (className == "Class")
        {
            importTableItem.ImportedObject = importPackage.FindClass(GetName(importTableItem.ObjectName));
        }
        else
        {
            importTableItem.ImportedObject = importPackage.FindObject(importFullName);
        }

        return importTableItem.ImportedObject;
    }

    internal UObject? FindObject(string importFullName)
    {
        var nameParts = importFullName.Split('.');
        var exportFullNameMatch = ExportTable.FirstOrDefault(x => GetName(x.ObjectName) == nameParts[^1] && GetFullName(x) == importFullName);
        if (exportFullNameMatch != null)
        {
            return exportFullNameMatch.Object;
        }

        var importFullNameMatch = ImportTable.FirstOrDefault(x => GetName(x.ObjectName) == nameParts[^1] && GetFullName(x) == importFullName);
        return importFullNameMatch?.ImportedObject;
    }

    /// <summary>
    ///     Native objects in a package are where the source package is the same package
    ///     where the import is defined (imported from itself)
    /// </summary>
    /// <returns></returns>
    private List<ImportTableItem> GetNativeObjectsFromImportsInPackage()
    {
        return ImportTable.Where(x =>
            GetName(GetImportPackage(x).ObjectName) == PackageName).ToList();
    }


    private void AddCoreNativeClasses()
    {
        ArgumentNullException.ThrowIfNull(PackageRoot);

        if (PackageName != "Core")
        {
            return;
        }


        var corePackageImport = ImportTable.FirstOrDefault(x => GetName(x.ObjectName) == "Core" && GetName(x.ClassName) == "Package");
        ArgumentNullException.ThrowIfNull(corePackageImport);
        corePackageImport.ImportedObject = PackageRoot;
        var nativeClassHelper = new NativeClassRegistrationHelper(PackageRoot);
        var nativeClasses = nativeClassHelper.GetNativeClasses(this);

        var coreFName = NameTable.FindIndex(x => x.Name == "Core");
        var classFName = NameTable.FindIndex(x => x.Name == "Class");
        UClass.StaticClass = nativeClasses["Class"];

        foreach (var (className, @class) in nativeClasses)
        {
            PackageClasses.Add(@class);
            var importItem = ImportTable.FirstOrDefault(x =>
                GetName(x.ObjectName) == @class.Name && x.ClassName.NameIndex == classFName && x.ClassPackage.NameIndex == coreFName);
            if (importItem != null)
            {
                importItem.ImportedObject = @class;
                continue;
            }

            var exportItem = ExportTable.FirstOrDefault(x =>
                GetName(x.ObjectName) == @class.Name && x.ClassIndex.Index == 0 && x.OuterIndex.Index == 0);
            if (exportItem != null)
            {
                exportItem.Object = @class;
            }
        }
    }

    /// <summary>
    ///     Returns a FName referencing the given name. If no such name is found in the name table, it will be added.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public FName GetOrAddName(string name)
    {
        var registeredName = NameTable.FindIndex(x => x.Name == name);
        if (registeredName != -1)
        {
            return new FName(registeredName);
        }

        NameTable.Add(new NameTableItem
        {
            Name = name,
            Flags = 0x7001000000000
        }); //flag might be wrong, but all the flags seems to be set to this in the packages I've looked at
        return new FName(NameTable.Count - 1);
    }


    public void LinkImports()
    {
        foreach (var import in ImportTable.Where(x => x.ImportedObject != null))
        {
            // link it
        }
    }

    public void CreateExportObjects()
    {
        for (var index = 0; index < ExportTable.Count; index++)
        {
            CreateExport(index);
        }
    }

    private void CreateExport(int index)
    {
        var exportItem = ExportTable[index];
        if (exportItem.Object is not null)
        {
            return;
        }

        var exportClass = FindExportClass(exportItem);
        var exportOuter = FindOuter(exportItem);
        var exportArchetype = FindExportArchetype(exportItem);
        UObject exportObject;
        if (exportClass == UClass.StaticClass)
        {
            var registeredClass = FindClass(GetName(exportItem.ObjectName));
            if (registeredClass is not null)
            {
                exportObject = registeredClass;
            }
            else
            {
                var superClass = FindSuperClass(exportItem);
                var newClass = new UClass(exportItem.ObjectName, exportClass, exportOuter, this, superClass);
                exportObject = newClass;
                PackageClasses.Add(newClass);
            }
        }
        else
        {
            exportObject = new UObject(exportItem.ObjectName, exportClass, exportOuter, this, exportArchetype);
        }

        exportItem.Object = exportObject;
    }

    private UClass? FindSuperClass(ExportTableItem exportItem)
    {
        if (exportItem.SuperIndex.Index == 0)
        {
            return null;
        }

        var classRef = GetObjectReference(exportItem.SuperIndex);
        return classRef switch
        {
            ImportTableItem import => import.ImportedObject as UClass,
            ExportTableItem export => export.Object as UClass,
            _ => throw new InvalidOperationException("Failed to find the class reference. Panic!")
        };
    }

    private UObject? FindExportArchetype(ExportTableItem exportItem)
    {
        if (exportItem.ArchetypeIndex.Index == 0)
        {
            return null;
        }

        var outerRef = GetObjectReference(exportItem.ArchetypeIndex);
        return outerRef switch
        {
            ImportTableItem import => import.ImportedObject,
            ExportTableItem export => export.Object,
            _ => null
        };
    }

    private UObject? FindOuter(IObjectResource exportItem)
    {
        if (exportItem.OuterIndex.Index == 0)
        {
            return PackageRoot;
        }

        var outerRef = GetObjectReference(exportItem.OuterIndex);
        return outerRef switch
        {
            ImportTableItem import => import.ImportedObject,
            ExportTableItem export => export.Object,
            _ => null
        };
    }

    private UClass? FindExportClass(ExportTableItem exportItem)
    {
        if (exportItem.ClassIndex.Index == 0)
        {
            return UClass.StaticClass;
        }

        var classRef = GetObjectReference(exportItem.ClassIndex);
        return classRef switch
        {
            ImportTableItem import => import.ImportedObject as UClass,
            ExportTableItem export => export.Object as UClass,
            _ => throw new InvalidOperationException("Failed to find the class reference. Panic!")
        };
    }

    public void GraphLink()
    {
        var graph = new ObjectDependencyGraph();
        graph.AddImportTableDependencies(ImportTable);
        graph.AddExportTableDependencies(ExportTable);

        var topoSort = graph.TopologicalSort().Select(x => new ObjectIndex(x));
        var visited = new HashSet<int>();
        foreach (var i in topoSort)
        {
            var refObj = GetObjectReference(i);
            switch (refObj)
            {
                case ExportTableItem export:
                    visited.Add(i.Index);
                    if (export.OuterIndex.Index != 0 && !visited.Contains(export.OuterIndex.Index))
                    {
                        throw new InvalidOperationException();
                    }

                    if (export.SuperIndex.Index != 0 && !visited.Contains(export.SuperIndex.Index))
                    {
                        throw new InvalidOperationException();
                    }

                    if (export.ArchetypeIndex.Index != 0 && !visited.Contains(export.ArchetypeIndex.Index))
                    {
                        throw new InvalidOperationException();
                    }

                    if (export.ClassIndex.Index != 0 && !visited.Contains(export.ClassIndex.Index))
                    {
                        throw new InvalidOperationException();
                    }

                    CreateExport(i.ExportIndex);

                    break;
                case ImportTableItem exportTable:
                    visited.Add(i.Index);
                    break;
            }
        }
    }
}