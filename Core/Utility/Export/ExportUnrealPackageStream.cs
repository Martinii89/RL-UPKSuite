using RlUpk.Core.Classes.Core;
using RlUpk.Core.Serialization;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types;
using RlUpk.Core.Types.PackageTables;

namespace RlUpk.Core.Utility.Export;

public class ExportUnrealPackageStream : UnrealPackageStream
{
    private readonly ExportTable _exportTable;
    private readonly ImportTable _importTable;

    public ExportUnrealPackageStream(Stream baseStream, IStreamSerializer<ObjectIndex> objectIndexSerializer, IStreamSerializer<FName> nameSerializer,
        UnrealPackage unrealPackage, ExportTable exportTable, ImportTable importTable) : base(baseStream, objectIndexSerializer, nameSerializer, unrealPackage)
    {
        _exportTable = exportTable;
        _importTable = importTable;
    }

    public override void WriteObject(UObject? obj)
    {
        if (obj == null)
        {
            _objectIndexSerializer.Serialize(BaseStream, new ObjectIndex());
            return;
        }

        if (obj.ExportTableItem is not null)
        {
            var exportIndex = _exportTable.FindIndex(o => o.Object == obj);
            if (exportIndex != -1)
            {
                _objectIndexSerializer.Serialize(BaseStream, new ObjectIndex(ObjectIndex.FromExportIndex(exportIndex)));
                return;
            }
        }
        else
        {
            var importIndex = _importTable.FindIndex(o => o.ImportedObject == obj);
            if (importIndex != -1)
            {
                _objectIndexSerializer.Serialize(BaseStream, new ObjectIndex(ObjectIndex.FromImportIndex(importIndex)));
                return;
            }
        }

        _objectIndexSerializer.Serialize(BaseStream, new ObjectIndex());
    }
}