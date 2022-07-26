﻿using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Utility;

public class PackageExporter
{
    private readonly Stream _exportStream;
    private readonly IStreamSerializer<ExportTableItem> _exporttTableItemSerializer;
    private readonly IStreamSerializer<FileSummary> _fileSummarySerializer;
    private readonly IStreamSerializer<ImportTableItem> _importTableItemSerializer;
    private readonly IStreamSerializer<FName> _nameSerializer;
    private readonly IStreamSerializer<NameTableItem> _nameTableItemSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;
    private readonly IUnrealPackageStream _outputPackageStream;

    private readonly UnrealPackage _package;

    public PackageExporter(UnrealPackage package, Stream exportStream, IStreamSerializer<FileSummary> fileSummarySerializer,
        IStreamSerializer<NameTableItem> nameTableItemSerializer, IStreamSerializer<ImportTableItem> importTableItemSerializer,
        IStreamSerializer<ExportTableItem> exporttTableItemSerializer, IStreamSerializer<ObjectIndex> objectIndexSerializer,
        IStreamSerializer<FName> nameSerializer)
    {
        _fileSummarySerializer = fileSummarySerializer;
        _nameTableItemSerializer = nameTableItemSerializer;
        _importTableItemSerializer = importTableItemSerializer;
        _exporttTableItemSerializer = exporttTableItemSerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _nameSerializer = nameSerializer;
        _exportStream = exportStream;
        _package = package;
        _outputPackageStream = new UnrealPackageStream(_exportStream, _objectIndexSerializer, _nameSerializer, _package);
    }

    public void ExportPackage()
    {
        ExportHeader();
        ExportNameTable();
        ExportImportTable();
        ExporExporttTable();
        ExportDummyDependsTable();
        ExportDummyThumbnailsTable();
        ExportObjectSerialData();
        // re-export export table once all the exported data is known
        _exportStream.Position = _package.Header.ExportOffset;
        ExporExporttTable();
    }

    /// <summary>
    ///     Writes the package header information to the start of the export stream
    /// </summary>
    public void ExportHeader()
    {
        _exportStream.Position = 0;
        var oldThumbnailTableOffset = _package.Header.ThumbnailTableOffset;
        _package.Header.ThumbnailTableOffset = 0;
        _fileSummarySerializer.Serialize(_exportStream, _package.Header);
        _package.Header.ThumbnailTableOffset = oldThumbnailTableOffset;
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
        var importObjects = _package.ImportTable.Select(x => x.ImportedObject).ToList();
        //var newImportTable = new ImportTable();
        _importTableItemSerializer.WriteTArray(_exportStream, _package.ImportTable.ToArray(), StreamSerializerForExtension.ArraySizeSerialization.NoSize);
    }

    /// <summary>
    ///     Writes the export table to the current position of the stream. Will not verify the stream offset to be the same as
    ///     <see cref="FileSummary.ExportOffset" />
    /// </summary>
    public void ExporExporttTable()
    {
        _exporttTableItemSerializer.WriteTArray(_exportStream, _package.ExportTable.ToArray(), StreamSerializerForExtension.ArraySizeSerialization.NoSize);
    }

    /// <summary>
    ///     Write a empty depends table to the stream. Will not verify the stream offset to be the same as
    ///     <see cref="FileSummary.DependsOffset" />
    /// </summary>
    public void ExportDummyDependsTable()
    {
        for (var i = 0; i < _package.Header.ImportCount; i++)
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

    public void ExportObjectSerialData()
    {
        var exports = _package.ExportTable;
        foreach (var export in exports)
        {
            var obj = export.Object;
            if (!obj.FullyDeserialized)
            {
                obj.Deserialize();
            }

            ArgumentNullException.ThrowIfNull(obj?.Serializer);
            var offset = _exportStream.Position;
            obj.Serializer.SerializeObject(obj, _outputPackageStream);
            var size = _exportStream.Position - offset;
            export.SerialOffset = offset;
            export.SerialSize = (int) size;
        }
    }
}