using Core.Types;

namespace Core.Utility;

public interface IImportResolver
{
    UnrealPackage? ResolveExportPackage(string packageName);
}