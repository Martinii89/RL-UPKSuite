using Core.Classes.Core;
using Core.Types;

namespace Core;

public interface INativeClassFactory
{
    Dictionary<string, UClass> GetNativeClasses(UnrealPackage package);
}