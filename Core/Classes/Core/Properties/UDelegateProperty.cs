﻿using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     Delegate property
/// </summary>
[NativeOnlyClass("Core", "DelegateProperty", "Property")]
public class UDelegateProperty : UProperty
{
    /// <inheritdoc />
    public UDelegateProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}