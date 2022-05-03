using System.Text;
using Core.Classes;
using Core.Classes.Core;
using Core.Types.PackageTables;
using Core.Utility;

namespace Core.Types;

/// <summary>
///     A UnrealPackage is the deserialized data from a UPK file. These files can contain all kinds of unreal object for a
///     game or specific maps..
/// </summary>
public class UnrealPackage
{
    public readonly List<UClass> PackageClasses = new();
    public IImportResolver? ImportResolver { get; set; }

    public PackageLoader? RootLoader { get; set; }

    public string PackageName { get; set; } = string.Empty;

    public UPackage? packageRoot { get; set; }

    //public UnrealPackage(IImportResolver importResolver)
    //{
    //    _importResolver = importResolver;
    //}

    /// <summary>
    ///     The header summarizes what the package contains and where in the file the different parts are located
    /// </summary>
    public FileSummary Header { get; set; } = new();

    /// <summary>
    ///     The name table contains all the names that this package references
    /// </summary>
    public NameTable NameTable { get; private set; } = new();

    /// <summary>
    ///     The import table references all the objects that this package depends on
    /// </summary>
    public ImportTable ImportTable { get; private set; } = new();

    /// <summary>
    ///     The export table contains all the objects that this package defines.
    /// </summary>
    public ExportTable ExportTable { get; private set; } = new();

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
    ///     Deserialized a package from a stream. Currently not fully implemented, it only deserializes the header and
    ///     name,import,export table. Objects are not constructed and imports are not linked with their package that exports
    ///     them.
    /// </summary>
    /// <param name="reader"></param>
    public void Deserialize(Stream reader)
    {
        Header.Deserialize(reader);

        NameTable = new NameTable(reader, Header.NameOffset, Header.NameCount);
        ImportTable = new ImportTable(reader, Header.ImportOffset, Header.ImportCount);
        ExportTable = new ExportTable(reader, Header.ExportOffset, Header.ExportCount);

        if (Header.CookerVersion != 0)
        {
            return;
        }

        reader.Position = Header.DependsOffset;
        DependsTable.InitializeSize(Header.ExportCount);
        DependsTable.Deserialize(reader);

        reader.Position = Header.ThumbnailTableOffset;
        ThumbnailTable.Deserialize(reader);

        if (reader is FileStream fileStream)
        {
            var fileName = Path.GetFileNameWithoutExtension(fileStream.Name);
            PackageName = fileName;
            if (PackageName == "Core")
            {
                InitNativeImportClasses();
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

    public UClass? FindClass(string className)
    {
        //foreach (var export in ExportTable)
        //{
        //    if (export.ClassIndex.Index == 0 && GetName(export.ObjectName) == className)
        //    {
        //        return export;
        //    }
        //}

        //foreach (var import in ImportTable)
        //{
        //    if (GetName(import.ClassName) == "Class" && GetName(import.ObjectName) == className)
        //    {
        //        return import;
        //    }
        //}

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

        return stringBuilder.ToString();
    }

    public UObject CreateExport(ExportTableItem exportTableItem)
    {
        throw new NotImplementedException();
        if (exportTableItem.Object != null)
        {
            return exportTableItem.Object;
        }

        UClass? exportClass = null;
        var outerReference = GetObjectReference(exportTableItem.OuterIndex);
        var @object = new UObject(exportTableItem.ObjectName, exportClass, null, this);
        exportTableItem.Object = @object;
        return @object;
    }

    public UObject? CreateImport(ImportTableItem importTableItem)
    {
        if (ImportResolver == null)
        {
            throw new InvalidDataException("Can't resolve imports without a import resolver");
        }

        var packageName = GetName(importTableItem.ClassPackage);
        var importPackage = ImportResolver.ResolveExportPackage(packageName);
        if (importPackage == null)
        {
            return null;
        }

        var className = GetName(importTableItem.ClassName);
        var importClass = importPackage.FindClass(className);
        //var @class = new UClass(importTableItem.ClassName, )

        var importOuter = GetObjectReference(importTableItem.OuterIndex);


        // Data Required
        // Name - yes
        // Class - almost
        // outer - almost
        // Owner - yes
        // Archetype - almost
        //new UObject(importTableItem.ObjectName, new UClass())


        //var export = importPackage.ExportTable.Exports.Where()


        return null;
    }


    public List<string> GetExportNamesAndOuters()
    {
        return ExportTable.Select(GetFullName).ToList();
    }

    /// <summary>
    ///     Initialize the native only classes defined in the Core package
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    public void InitNativeImportClasses()
    {
        var nativeClasses = ImportTable.Where(x => GetName(x.ClassName) == "Class" && GetName(x.ClassPackage) == PackageName).ToList();
        if (nativeClasses.Count == 0)
        {
            return;
        }

        if (PackageName != "Core")
        {
            throw new NotImplementedException("native class init not implemented for non core package");
        }

        var names = nativeClasses.Select(x => GetName(x.ObjectName));


        // Register Class and Package classes
        var classClassImport = nativeClasses.FirstOrDefault(x => GetName(x.ObjectName) == "Class");
        var corePackageImport = ImportTable.FirstOrDefault(x => GetName(x.ObjectName) == "Core" && GetName(x.ClassName) == "Package");

        if (corePackageImport == null)
        {
            throw new NullReferenceException(nameof(corePackageImport));
        }

        if (classClassImport == null)
        {
            throw new NullReferenceException(nameof(classClassImport));
        }

        var rootPackage = new UObject(corePackageImport.ObjectName, null, null, this);
        var @class = new UClass(classClassImport.ObjectName, null, rootPackage, this);
        var package = new UClass(corePackageImport.ClassName, @class, rootPackage, this);
        rootPackage.Class = package;
        PackageClasses.Add(@class);
        PackageClasses.Add(package);

        classClassImport.ImportedObject = @class;
        corePackageImport.ImportedObject = rootPackage;

        foreach (var nativeClass in nativeClasses)
        {
            if (FindClass(GetName(nativeClass.ObjectName)) != null)
            {
                continue;
            }

            var newClass = new UClass(nativeClass.ObjectName, @class, rootPackage, this);
            PackageClasses.Add(newClass);
        }
        //var structClass = new UClass()
    }


    public void InitializeExportClasses()
    {
        var exportClasses = ExportTable.Where(x => x.ClassIndex.Index == 0);
        UClass? @class;
        @class = PackageName == "Core" ? FindClass("Class") : ImportResolver?.ResolveExportPackage("Core")?.FindClass("Class");

        if (@class == null)
        {
            throw new NullReferenceException("Failed to initialize classes");
        }

        foreach (var exportClass in exportClasses)
        {
            var exportName = GetName(exportClass.ObjectName);
            if (exportClass.OuterIndex.Index != 0)
            {
                throw new NotImplementedException("Resolving outer object not implemented yet");
            }

            if (exportClass.SuperIndex.GetReferencedTable() == ObjectIndex.ReferencedTable.Import)
            {
                throw new NotImplementedException("Resolving import super classes not implemented yet");
            }

            var registeredClas = FindClass(exportName);
            if (registeredClas != null)
            {
                exportClass.Object = registeredClas;
                continue;
            }

            var superRef = GetObjectReference(exportClass.SuperIndex);
            var superClass = superRef == null ? null : FindClass(GetName(superRef.ObjectName));
            var newClass = new UClass(exportClass.ObjectName, @class, packageRoot, this, superClass);
            exportClass.Object = newClass;
            PackageClasses.Add(newClass);
        }
    }

    public void AddNativeClasses()
    {
        if (PackageName != "Core")
        {
            return;
        }

        var corePackageImport = ImportTable.FirstOrDefault(x => GetName(x.ObjectName) == "Core" && GetName(x.ClassName) == "Package");
        packageRoot = new UPackage(corePackageImport.ObjectName, null, null, this);
        corePackageImport.ImportedObject = packageRoot;
        var nativeClassHelper = new NativeClassRegistrationHelper(packageRoot);
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
            }
            else
            {
                var exportItem = ExportTable.FirstOrDefault(x =>
                    GetName(x.ObjectName) == @class.Name && x.ClassIndex.Index == 0 && x.OuterIndex.Index == 0);
                if (exportItem != null)
                {
                    exportItem.Object = @class;
                }
            }
        }
    }

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

    private UPackage CreatePackageRoot(string packageName)
    {
        var fname = GetOrAddName(packageName);
        return new UPackage(fname, UClass.StaticClass, null, this);
    }

    public void LinkImports()
    {
        foreach (var import in ImportTable.Where(x => x.ImportedObject != null))
        {
            // link it
        }
    }

    public void LinkExports()
    {
        for (var index = 0; index < ExportTable.Count; index++)
        {
            LinkExport(index);
        }
    }

    private void LinkExport(int index)
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
            return packageRoot;
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
        var graph = new Graph();

        // Add imports to dependency graph
        for (var index = 0; index < ImportTable.Count; index++)
        {
            var import = ImportTable[index];
            var outerIndex = import.OuterIndex.Index;
            if (outerIndex != 0)
            {
                graph.AddEdge(outerIndex, ObjectIndex.FromImportIndex(index), EdgeType.Outer);
            }
        }

        // Add exports to dependency graph
        for (var index = 0; index < ExportTable.Count; index++)
        {
            var export = ExportTable[index];
            var outer = export.OuterIndex.Index;
            if (outer != 0)
            {
                graph.AddEdge(outer, ObjectIndex.FromExportIndex(index), EdgeType.Outer);
            }

            var super = export.SuperIndex.Index;
            if (super != 0)
            {
                graph.AddEdge(super, ObjectIndex.FromExportIndex(index), EdgeType.Super);
            }

            var archetype = export.ArchetypeIndex.Index;
            if (archetype != 0)
            {
                graph.AddEdge(archetype, ObjectIndex.FromExportIndex(index), EdgeType.Archetype);
            }

            var @class = export.ClassIndex.Index;
            if (@class != 0)
            {
                graph.AddEdge(@class, ObjectIndex.FromExportIndex(index), EdgeType.Class);
            }
        }

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

                    LinkExport(i.ExportIndex);

                    break;
                case ImportTableItem exportTable:
                    visited.Add(i.Index);
                    break;
            }
        }
    }
}