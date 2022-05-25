using System.Diagnostics;
using System.Text;
using Core.Classes;
using Core.Classes.Core;
using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;
using Core.Utility;

namespace Core.Types;

public class UnrealPackageOptions
{
    public UnrealPackageOptions(IStreamSerializerFor<UnrealPackage> serializer, string packageName, IPackageCache? packageCache = null,
        IObjectSerializerFactory? objectSerializerFactory = null)
    {
        Serializer = serializer;
        PackageName = packageName;
        PackageCache = packageCache;
        ObjectSerializerFactory = objectSerializerFactory;
    }

    /// <summary>
    ///     Serializer to use with the package
    /// </summary>
    public IStreamSerializerFor<UnrealPackage> Serializer { get; set; }

    /// <summary>
    ///     The name of the package
    /// </summary>
    public string PackageName { get; set; }

    /// <summary>
    ///     Cache used to resolve\cache import packages
    /// </summary>
    public IPackageCache? PackageCache { get; set; }

    /// <summary>
    ///     Factory used to create serializers for all UObjects in the package
    /// </summary>
    public IObjectSerializerFactory? ObjectSerializerFactory { get; set; }
}

/// <summary>
///     A UnrealPackage is the deserialized data from a UPK file. These files can contain all kinds of unreal object for a
///     game or specific maps..
/// </summary>
public class UnrealPackage
{
    private const string CorePackageName = "Core";

    /// <summary>
    ///     A List of classes defined in this package.
    ///     TODO: Reconsider this and instead use a shared common dictionary with full name classes
    /// </summary>
    public readonly List<UClass> PackageClasses = new();

    /// <summary>
    ///     A Import resolver use to resolve the import objects
    /// </summary>
    public IPackageCache? ImportResolver { get; set; }

    /// <summary>
    ///     Factory used to create serializers for all UObjects in the package
    /// </summary>
    public IObjectSerializerFactory? ObjectSerializerFactory { get; set; }

    /// <summary>
    ///     The root. (May be removed)
    /// </summary>
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
    ///     The original data stream of the package
    /// </summary>
    public Stream? PackageStream { get; set; }

    /// <summary>
    ///     Helper constructor to create and initialize a package from a <see cref="IStreamSerializerFor{T}" />
    /// </summary>
    /// <param name="stream">The package stream</param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static UnrealPackage DeserializeAndInitialize(Stream stream, UnrealPackageOptions options)
    {
        var package = options.Serializer.Deserialize(stream);
        package.PackageStream = stream;
        package.ImportResolver = options.PackageCache;
        package.ObjectSerializerFactory = options.ObjectSerializerFactory;
        package.PostDeserializeInitialize(options.PackageName);
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
            AddNativeClassesFromImports();
        }

        var packageClass = FindClass("Package");
        PackageRoot.Class = packageClass;
    }

    private void AddNativeClassesFromImports()
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
    public IObjectResource? GetObjectReference(ObjectIndex index)
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
    ///     Returns the ObjectResource of the ObjectIndex. This will either be a <see cref="ImportTableItem" /> from the import
    ///     table or a <see cref="ExportTableItem" /> from the export table
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public UObject? GetObject(ObjectIndex index)
    {
        return index.GetReferencedTable() switch
        {
            ObjectIndex.ReferencedTable.Null => null,
            ObjectIndex.ReferencedTable.Import => ImportTable[index.ImportIndex].ImportedObject,
            ObjectIndex.ReferencedTable.Export => ExportTable[index.ExportIndex].Object,
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
#if DEBUG
        if (name.NameIndex >= NameTable.Count || name.NameIndex < 0)
        {
            Debugger.Break();
            //throw new IndexOutOfRangeException($"Invalid FName index {name.NameIndex}");
        }
#endif
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
        var name = GetName(objectResource.ClassName);
        if ((name == "Package" || name == "None") && objectResource.OuterIndex.Index == 0)
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

    /// <summary>
    ///     Resolves and creates the object for a import.
    /// </summary>
    /// <param name="importTableItem"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public UObject? CreateImport(ImportTableItem importTableItem)
    {
        if (ImportResolver == null)
        {
            throw new InvalidDataException("Can't resolve imports without a import resolver");
        }

        // Already created
        if (importTableItem.ImportedObject is not null)
        {
            return importTableItem.ImportedObject;
        }

        // Top level package import
        if (importTableItem.OuterIndex.Index == 0 && GetName(importTableItem.ClassName) == "Package")
        {
            importTableItem.ImportedObject = new UPackage(importTableItem.ObjectName, FindClass("Package"), null, this);
            return importTableItem.ImportedObject;
        }

        var importFullName = GetFullName(importTableItem);
        var importPackageName = importFullName.Split(".")[0];

        var importPackage = ImportResolver.ResolveExportPackage(importPackageName);
        // Object contained in a self imported package? Not sure what these really are.
        // Maybe it's a package defined in a unknown file somewhere else. 
        if (importPackage == null)
        {
            importTableItem.ImportedObject = CreateInternalImport(importTableItem);
            return importTableItem.ImportedObject;
        }

        // Recursively import the outer objects
        var importOuter = GetObjectReference(importTableItem.OuterIndex);
        if (importOuter is ImportTableItem { ImportedObject: null } import)
        {
            import.ImportedObject = CreateImport(import);
        }

        // Fetch the object\class from the package that defines it.
        var className = GetName(importTableItem.ClassName);
        if (className == "Class")
        {
            importTableItem.ImportedObject = importPackage.FindClass(GetName(importTableItem.ObjectName));
            if (importTableItem.ImportedObject == null)
            {
                //most likely a native class. Stub it
                var cls = new UClass(importTableItem.ObjectName, UClass.StaticClass, importPackage.PackageRoot, this);
                importTableItem.ImportedObject = cls;
                importPackage.PackageClasses.Add(cls);
                //panic
            }
        }
        else
        {
            importTableItem.ImportedObject = importPackage.FindObject(importFullName);
        }

        // We failed :(
        if (importTableItem.ImportedObject == null)
        {
            //probably a native class field?
            importTableItem.ImportedObject = CreateInternalImport(importTableItem);
        }


        return importTableItem.ImportedObject;
    }

    /// <summary>
    ///     Creates a stub import object for a object that we failed to resolve the exporting package for
    /// </summary>
    /// <param name="import"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private UObject CreateInternalImport(ImportTableItem import)
    {
        var classPackageName = GetName(import.ClassPackage);
        var classPackage = ImportResolver!.ResolveExportPackage(classPackageName);
        UClass? cls = null;
        if (classPackage != null)
        {
            cls = classPackage.FindClass(GetName(import.ClassName));
        }

        var outerRef = GetObjectReference(import.OuterIndex);
        var outer = outerRef switch
        {
            ImportTableItem importOuter => importOuter.ImportedObject,
            ExportTableItem export => export.Object,
            _ => null
        };
        var obj = new UObject(import.ObjectName, cls, outer, this);
        return obj;
    }

    internal UObject? FindObject(string importFullName)
    {
        var nameParts = importFullName.Split('.');
        foreach (var exportItem in ExportTable)
        {
            if (GetName(exportItem.ObjectName) != nameParts[^1])
            {
                continue;
            }

            if (GetFullName(exportItem) == importFullName)
            {
                return exportItem.Object;
            }
        }

        foreach (var importTableItem in ImportTable)
        {
            if (GetName(importTableItem.ObjectName) != nameParts[^1])
            {
                continue;
            }

            if (GetFullName(importTableItem) == importFullName)
            {
                return importTableItem.ImportedObject;
            }
        }

        return null;
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

        foreach (var (_, @class) in nativeClasses)
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
            if (exportItem == null)
            {
                continue;
            }

            exportItem.Object = @class;
            @class.ExportTableItem = exportItem;
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

        NameTable.Add(new NameTableItem(name, 0x7001000000000)); //flag might be wrong, but all the flags seems to be set to this in the packages I've looked at
        return new FName(NameTable.Count - 1);
    }


    /// <summary>
    ///     Iterates the export table and create all the export objects.
    /// </summary>
    public void CreateExportObjects()
    {
        for (var index = 0; index < ExportTable.Count; index++)
        {
            CreateExport(index);
        }
    }

    private void CreateExport(ExportTableItem exportItem)
    {
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
            if (exportClass is null)
            {
                exportObject = new UObject(exportItem.ObjectName, exportClass, exportOuter, this, exportArchetype);
            }
            else
            {
                exportObject = exportClass.NewInstance(exportItem.ObjectName, exportOuter, this, exportArchetype);
            }
        }

        exportObject.ExportTableItem = exportItem;
        exportItem.Object = exportObject;
    }

    private void CreateExport(int index)
    {
        var exportItem = ExportTable[index];
        CreateExport(exportItem);
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

    /// <summary>
    ///     Links all the objects defined in the package. Uses a ObjectDependencyGraph to make sure they are created and linked
    ///     up in the correct order. (No object should be created before it's dependencies)
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
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
                case ImportTableItem:
                    visited.Add(i.Index);
                    break;
            }
        }
    }


    /// <summary>
    ///     Create the object for a import\export table item
    /// </summary>
    /// <param name="objectResource"></param>
    public void CreateObject(IObjectResource objectResource)
    {
        switch (objectResource)
        {
            case ExportTableItem export:
                CreateExport(export);
                break;
            case ImportTableItem import:
                CreateImport(import);
                break;
        }
    }
}