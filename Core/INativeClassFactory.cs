using RlUpk.Core.Classes.Core;
using RlUpk.Core.Types;

namespace RlUpk.Core;

public interface INativeClassFactory
{
    Dictionary<string, UClass> GetNativeClasses(UnrealPackage package);

    UClass? StaticClass { get; }
}