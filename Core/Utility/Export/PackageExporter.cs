using Core.Classes;
using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Utility.Export;

public class PackageExporter
{
    private readonly ExportTable _exportExportTable;
    private readonly FileSummary _exportHeader;
    private readonly ImportTable _exportImportTable;
    private readonly Stream _exportStream;
    private readonly IStreamSerializer<ExportTableItem> _exportTableItemSerializer;
    private readonly IStreamSerializer<FileSummary> _fileSummarySerializer;
    private readonly IStreamSerializer<ImportTableItem> _importTableItemSerializer;
    private readonly IStreamSerializer<FName> _nameSerializer;
    private readonly IStreamSerializer<NameTableItem> _nameTableItemSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;
    private readonly IUnrealPackageStream _outputPackageStream;
    private readonly UnrealPackage _package;

    public PackageExporter(UnrealPackage package, Stream exportStream, IStreamSerializer<FileSummary> fileSummarySerializer,
        IStreamSerializer<NameTableItem> nameTableItemSerializer, IStreamSerializer<ImportTableItem> importTableItemSerializer,
        IStreamSerializer<ExportTableItem> exportTableItemSerializer, IStreamSerializer<ObjectIndex> objectIndexSerializer,
        IStreamSerializer<FName> nameSerializer)
    {
        _fileSummarySerializer = fileSummarySerializer;
        _nameTableItemSerializer = nameTableItemSerializer;
        _importTableItemSerializer = importTableItemSerializer;
        _exportTableItemSerializer = exportTableItemSerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _nameSerializer = nameSerializer;
        _exportStream = exportStream;
        _package = package;
        _exportHeader = CopyHeader(_package.Header);
        _exportExportTable = CopyExportTable(_package.ExportTable);
        _exportImportTable = CopyImportTable(_package.ImportTable);
        RemoveNullObjects(_exportImportTable, _exportExportTable);
        ModifyHeaderFieldsForExport(_exportHeader);
        ModifyExportTableFieldsForExport(_exportExportTable);
        FixObjectIndexReferences(_exportImportTable, _exportExportTable);
        _outputPackageStream =
            new ExportUnrealPackageStream(_exportStream, _objectIndexSerializer, _nameSerializer, _package, _exportExportTable, _exportImportTable);
    }

    private void FixObjectIndexReferences(ImportTable importTable, ExportTable exportTable)
    {
        foreach (var import in importTable)
        {
            var obj = import.ImportedObject;
            ArgumentNullException.ThrowIfNull(obj);
            if (import.OuterIndex.Index != 0)
            {
                import.OuterIndex = FindObjectIndex(obj.Outer);
            }
        }

        foreach (var export in exportTable)
        {
            var obj = export.Object;
            ArgumentNullException.ThrowIfNull(obj);
            if (export.ClassIndex.Index != 0)
            {
                export.ClassIndex = FindObjectIndex(obj.Class);
            }

            if (export.SuperIndex.Index != 0)
            {
                ArgumentNullException.ThrowIfNull(obj.Class);
                export.SuperIndex = FindObjectIndex(obj.Class.SuperClass);
            }

            if (export.OuterIndex.Index != 0)
            {
                export.OuterIndex = FindObjectIndex(obj.Outer);
            }

            if (export.ArchetypeIndex.Index != 0)
            {
                export.ArchetypeIndex = FindObjectIndex(obj.ObjectArchetype);
            }
        }
    }


    private ObjectIndex FindObjectIndex(UObject? obj)
    {
        if (obj == null)
        {
            return new ObjectIndex();
        }

        var exportIndex = _exportExportTable.FindIndex(o => o.Object == obj);
        if (exportIndex != -1)
        {
            return new ObjectIndex(ObjectIndex.FromExportIndex(exportIndex));
        }

        var importIndex = _exportImportTable.FindIndex(o => o.ImportedObject == obj);
        if (importIndex != -1)
        {
            return new ObjectIndex(ObjectIndex.FromImportIndex(importIndex));
        }

        return new ObjectIndex();
    }

    private void RemoveNullObjects(ImportTable importTable, ExportTable exportTable)
    {
        var noneFName = _package.GetFName("None");
        importTable.RemoveAll(x => Equals(x.ObjectName, noneFName) && Equals(x.ClassName, noneFName) && Equals(x.ClassPackage, noneFName));
        exportTable.RemoveAll(x => x.SerialSize == 0);
        //var index = 140; ok, but material functions crashes
        //var index = 141; // crash
        var index = 141;
        exportTable.RemoveRange(index, exportTable.Count - index);
    }

    private void ModifyExportTableFieldsForExport(ExportTable exportTable)
    {
        foreach (var export in exportTable)
        {
            switch (export.Object)
            {
                case null:
                    continue;
                case UPackage:
                    export.ObjectFlags = 0x7000400000000u;
                    export.PackageFlags = 1;
                    break;
                case UMaterial:
                case USkeletalMesh:
                case UStaticMesh:
                case UTexture:
                    export.ObjectFlags = 0xF000400000000u;
                    export.PackageFlags = 0;
                    break;
                default:
                    export.ObjectFlags = 0xF000400000400;
                    export.PackageFlags = 0;
                    break;
            }
        }
    }

    private void ModifyHeaderFieldsForExport(FileSummary exportHeader)
    {
        exportHeader.LicenseeVersion = 0;
        exportHeader.ThumbnailTableOffset = 0;
        exportHeader.CookerVersion = 0;
        exportHeader.EngineVersion = 12791;
        exportHeader.PackageFlags = 1;
        /*
        //var flags = (PackageFlags) exportHeader.PackageFlags;
        //flags &= ~PackageFlags.PKG_Cooked;
        //flags &= ~PackageFlags.PKG_StoreCompressed;

        ////var a = flags.HasFlag(PackageFlags.PKG_AllowDownload);
        ////var a1 = flags.HasFlag(PackageFlags.PKG_ClientOptional);
        ////var a2 = flags.HasFlag(PackageFlags.PKG_ServerSideOnly);
        ////var a3 = flags.HasFlag(PackageFlags.PKG_Cooked);
        ////var a4 = flags.HasFlag(PackageFlags.PKG_Unsecure);
        ////var a5 = flags.HasFlag(PackageFlags.PKG_SavedWithNewerVersion);
        ////var a6 = flags.HasFlag(PackageFlags.PKG_Need);
        ////var a7 = flags.HasFlag(PackageFlags.PKG_Compiling);
        ////var a8 = flags.HasFlag(PackageFlags.PKG_ContainsMap);
        ////var a9 = flags.HasFlag(PackageFlags.PKG_Trash);
        ////var a10 = flags.HasFlag(PackageFlags.PKG_DisallowLazyLoading);
        ////var a11 = flags.HasFlag(PackageFlags.PKG_PlayInEditor);
        ////var a12 = flags.HasFlag(PackageFlags.PKG_ContainsScript);
        ////var a13 = flags.HasFlag(PackageFlags.PKG_ContainsDebugInfo);
        ////var a14 = flags.HasFlag(PackageFlags.PKG_RequireImportsAlreadyLoaded);
        ////var a15 = flags.HasFlag(PackageFlags.PKG_SelfContainedLighting);
        ////var a16 = flags.HasFlag(PackageFlags.PKG_StoreCompressed);
        ////var a17 = flags.HasFlag(PackageFlags.PKG_StoreFullyCompressed);
        ////var a18 = flags.HasFlag(PackageFlags.PKG_ContainsInlinedShaders);
        ////var a19 = flags.HasFlag(PackageFlags.PKG_ContainsFaceFXData);
        ////var a20 = flags.HasFlag(PackageFlags.PKG_NoExportAllowed);
        ////var a21 = flags.HasFlag(PackageFlags.PKG_StrippedSource);
        //exportHeader.PackageFlags = (uint) flags;
        */
    }

    private FileSummary CopyHeader(FileSummary header)
    {
        var stream = new MemoryStream();
        _fileSummarySerializer.Serialize(stream, header);
        stream.Position = 0;
        return _fileSummarySerializer.Deserialize(stream);
    }

    private ImportTable CopyImportTable(ImportTable importTable)
    {
        var stream = new MemoryStream();
        _importTableItemSerializer.WriteTArray(stream, importTable.ToArray(), StreamSerializerForExtension.ArraySizeSerialization.NoSize);
        stream.Position = 0;
        var newTable = new ImportTable(_importTableItemSerializer, stream, importTable.Count);
        for (var i = 0; i < newTable.Count; i++)
        {
            newTable[i].ImportedObject = importTable[i].ImportedObject;
        }

        return newTable;
    }

    private ExportTable CopyExportTable(ExportTable exportTable)
    {
        var stream = new MemoryStream();
        _exportTableItemSerializer.WriteTArray(stream, exportTable.ToArray(), StreamSerializerForExtension.ArraySizeSerialization.NoSize);
        stream.Position = 0;
        var newTable = new ExportTable(_exportTableItemSerializer, stream, exportTable.Count);
        for (var i = 0; i < newTable.Count; i++)
        {
            newTable[i].Object = exportTable[i].Object;
        }

        return newTable;
    }

    /// <summary>
    ///     Write the complete package to the output stream
    /// </summary>
    public void ExportPackage(IObjectSerializerFactory? serializerFactory = null)
    {
        ExportHeader();
        _exportHeader.NameOffset = (int) _exportStream.Position;
        ExportNameTable();
        _exportHeader.ImportOffset = (int) _exportStream.Position;
        _exportHeader.ImportCount = _exportImportTable.Count;
        ExportImportTable();
        _exportHeader.ExportOffset = (int) _exportStream.Position;
        _exportHeader.ExportCount = _exportExportTable.Count;
        ExporExporttTable();
        _exportHeader.DependsOffset = (int) _exportStream.Position;
        ExportDummyDependsTable();
        _exportHeader.ThumbnailTableOffset = 0;
        ExportDummyThumbnailsTable();
        _exportHeader.TotalHeaderSize = (int) _exportStream.Position;
        ExportObjectSerialData(serializerFactory);
        // re-export export table once all the exported data is known
        _exportStream.Position = _exportHeader.ExportOffset;
        ExporExporttTable();
        // re-export the header once all the header data is known
        ExportHeader();
    }

    /// <summary>
    ///     Writes the package header information to the start of the export stream
    /// </summary>
    public void ExportHeader()
    {
        _exportStream.Position = 0;
        _fileSummarySerializer.Serialize(_exportStream, _exportHeader);
    }

    /// <summary>
    ///     Writes the name table to the current position of the stream. Will not verify the stream offset to be the same as
    ///     <see cref="FileSummary.NameOffset" />
    /// </summary>
    public void ExportNameTable()
    {
        _nameTableItemSerializer.WriteTArray(_exportStream, _package.NameTable.ToArray(), StreamSerializerForExtension.ArraySizeSerialization.NoSize);
    }

    /// <summary>
    ///     Writes the import table to the current position of the stream. Will not verify the stream offset to be the same as
    ///     <see cref="FileSummary.ImportOffset" />
    /// </summary>
    public void ExportImportTable()
    {
        //null out any outers that are exported from this package. Some kind of artifact from forced exports \ cooked packages
        // TODO: Convert these exports with internal imports into pure imports
        foreach (var import in _exportImportTable)
        {
            var importImportedObject = import.ImportedObject;
            if (importImportedObject is null)
            {
                continue;
            }

            if (importImportedObject.Outer?.OwnerPackage == _package && importImportedObject.Outer?.ExportTableItem is not null)
            {
                import.OuterIndex = new ObjectIndex();
            }
        }

        _importTableItemSerializer.WriteTArray(_exportStream, _exportImportTable.ToArray(), StreamSerializerForExtension.ArraySizeSerialization.NoSize);
    }

    /// <summary>
    ///     Writes the export table to the current position of the stream. Will not verify the stream offset to be the same as
    ///     <see cref="FileSummary.ExportOffset" />
    /// </summary>
    public void ExporExporttTable()
    {
        _exportTableItemSerializer.WriteTArray(_exportStream, _exportExportTable.ToArray(), StreamSerializerForExtension.ArraySizeSerialization.NoSize);
    }

    /// <summary>
    ///     Write a empty depends table to the stream. Will not verify the stream offset to be the same as
    ///     <see cref="FileSummary.DependsOffset" />
    /// </summary>
    public void ExportDummyDependsTable()
    {
        for (var i = 0; i < _exportExportTable.Count; i++)
        {
            _exportStream.WriteInt32(0);
        }
    }

    /// <summary>
    ///     Does nothing as this exporter is made to skip thumbnails all together. The thumbnail data would be serialized after
    ///     the export table. after the thumbnail data the thumbnail table would be serialized.
    ///     The reason for serializing the table after data, is that the table requires the data offsets to be known
    /// </summary>
    public void ExportDummyThumbnailsTable()
    {
    }

    /// <summary>
    ///     Write the object serial data to the output stream. Will throw if any objects serializers fails to resolve.
    ///     Overwrite the export table entries of the object package with new offsets and sizes.
    /// </summary>
    public void ExportObjectSerialData(IObjectSerializerFactory? objectSerializerFactory = null)
    {
        var exports = _exportExportTable;
        foreach (var export in exports)
        {
            var obj = export.Object;
            if (!obj.FullyDeserialized)
            {
                obj.Deserialize();
            }

            var offset = _exportStream.Position;
            var serializer = GetObjectSerializer(obj, objectSerializerFactory);
            serializer.SerializeObject(obj, _outputPackageStream);
            var size = _exportStream.Position - offset;
            export.SerialOffset = offset;
            export.SerialSize = (int) size;
        }
    }

    private static IObjectSerializer GetObjectSerializer(UObject obj, IObjectSerializerFactory? factory)
    {
        if (factory != null)
        {
            var t = obj.GetType();
            IObjectSerializer? serializer = null;
            while (serializer is null && t is not null)
            {
                serializer = factory.GetSerializer(t);
                t = t.BaseType;
            }

            ArgumentNullException.ThrowIfNull(serializer);
            return serializer;
        }

        ArgumentNullException.ThrowIfNull(obj?.Serializer);
        return obj.Serializer;
    }
}