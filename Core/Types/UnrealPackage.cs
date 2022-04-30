using System.Text;
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

            var superRef = GetObjectReference(exportClass.SuperIndex);

            var superClass = superRef == null ? null : FindClass(GetName(superRef.ObjectName));
            var newClass = new UClass(exportClass.ObjectName, @class, null, this, superClass);
            PackageClasses.Add(newClass);
        }
    }
}