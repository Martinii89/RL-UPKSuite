﻿using System.Diagnostics;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Utility;

/// <summary>
///     Helper class to construct a complete dependency graph, including all required objects from imported packages
/// </summary>
public class CrossPackageDependencyGraph
{
    private readonly Dictionary<PackageObjectReference, HashSet<Edge>> _adj = new();
    private readonly Dictionary<string, PackageObjectReference> _fullNameToReferenceCache = new();

    private readonly IPackageCache _packagePackageCache;

    /// <summary>
    ///     Create a package dependency graph. A import resolver is required to resolve import object dependencies correctly.
    /// </summary>
    /// <param name="packagePackageCache"></param>
    public CrossPackageDependencyGraph(IPackageCache packagePackageCache)
    {
        _packagePackageCache = packagePackageCache;
    }


    /// <summary>
    ///     How many objects in the graph
    /// </summary>
    public int NodeCount => _adj.Count;

    public int CacheHits { get; set; }
    public int CacheMisses { get; set; }


    /// <summary>
    ///     Add a object to the graph.
    /// </summary>
    /// <param name="node">The new node</param>
    public void AddNode(PackageObjectReference node)
    {
        if (_adj.ContainsKey(node))
        {
            // Fail silently.
            return;
        }

        _adj[node] = new HashSet<Edge>();
    }

    private static bool ImportIsNative(ImportTableItem importItem, UnrealPackage package)
    {
        return package.GetName(package.GetImportPackage(importItem).ObjectName) == package.PackageName;
    }


    private PackageObjectReference ResolveImportObjectReference(ImportTableItem import, UnrealPackage objPackage)
    {
        var importPackageReference = objPackage.GetImportPackage(import);
        var packageName = objPackage.GetName(importPackageReference.ObjectName);
        var importPackage = _packagePackageCache.ResolveExportPackage(packageName);
        if (importPackage is null)
        {
            // null reference
            return new PackageObjectReference();
        }

        var objName = objPackage.GetName(import.ObjectName);
        var fullName = objPackage.GetFullName(import);
        if (_fullNameToReferenceCache.ContainsKey(fullName))
        {
            CacheHits++;
            return _fullNameToReferenceCache[fullName];
        }

        CacheMisses++;

        for (var index = 0; index < importPackage.ExportTable.Count; index++)
        {
            var exportTableItem = importPackage.ExportTable[index];
            if (importPackage.GetName(exportTableItem.ObjectName) != objName || importPackage.GetFullName(exportTableItem) != fullName)
            {
                continue;
            }

            var exportIndex = new ObjectIndex(ObjectIndex.FromExportIndex(index));
            var resolveImportObjectReference = new PackageObjectReference(packageName, exportIndex);
            _fullNameToReferenceCache.Add(fullName, resolveImportObjectReference);
            return resolveImportObjectReference;
        }

        for (var index = 0; index < importPackage.ImportTable.Count; index++)
        {
            var x = importPackage.ImportTable[index];
            if (importPackage.GetName(x.ObjectName) != objName || importPackage.GetFullName(x) != fullName)
            {
                continue;
            }

            var importIndex = new ObjectIndex(ObjectIndex.FromImportIndex(index));
            var resolveImportObjectReference = new PackageObjectReference(packageName, importIndex);
            _fullNameToReferenceCache.Add(fullName, resolveImportObjectReference);
            return resolveImportObjectReference;
        }


        var nativeClass = importPackage.FindClass(objPackage.GetName(import.ObjectName));
        if (nativeClass is not null)
        {
            var resolveImportObjectReference = new PackageObjectReference(packageName, nativeClass);
            _fullNameToReferenceCache.Add(fullName, resolveImportObjectReference);
            return resolveImportObjectReference;
        }

        // null reference
        var packageObjectReference = new PackageObjectReference();
        _fullNameToReferenceCache.Add(fullName, packageObjectReference);
        return packageObjectReference;
    }

    /// <summary>
    ///     Adds the given object to the dependency graph. Throw if any dependent objects fails to resolve
    /// </summary>
    /// <param name="objReference"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddObjectDependencies(PackageObjectReference objReference)
    {
        var objQueue = new Queue<PackageObjectReference>();
        objQueue.Enqueue(objReference);
        AddNode(objReference);

        while (objQueue.Count != 0)
        {
            var currentObj = objQueue.Dequeue();
            var objPackage = _packagePackageCache.ResolveExportPackage(currentObj.PackageName);
            if (objPackage is null)
            {
                Debugger.Break();
                throw new InvalidOperationException($"Can't find the package to resolve dependencies for {currentObj.PackageName}");
            }

            var obj = objPackage.GetObjectReference(currentObj.ObjectIndex);
            switch (obj)
            {
                case ImportTableItem import:
                {
                    AddImportDependencies(import, currentObj, objQueue, objPackage);
                    break;
                }
                case ExportTableItem export:
                {
                    AddImportDependencies(export, currentObj, objQueue);
                    break;
                }
            }
        }
    }

    private void AddImportDependencies(ImportTableItem import, PackageObjectReference currentObj, Queue<PackageObjectReference> objQueue,
        UnrealPackage objPackage)
    {
        var clzReference = ResolveImportClassReference(import, objPackage);
        if (!clzReference.IsNull() && clzReference.NativeClass is null && !_adj.ContainsKey(clzReference))
        {
            objQueue.Enqueue(clzReference);
        }

        // class of UClass could return itself
        if (!clzReference.IsNull() && clzReference != currentObj)
        {
            AddEdge(clzReference, currentObj);
        }


        if (import.OuterIndex.Index == 0)
        {
            return;
        }

        var outerReference = new PackageObjectReference(currentObj.PackageName, import.OuterIndex);
        if (!_adj.ContainsKey(outerReference))
        {
            objQueue.Enqueue(outerReference);
        }

        AddEdge(outerReference, currentObj);


        // Native imports has no export reference to resolve
        if (ImportIsNative(import, objPackage))
        {
            return;
        }

        var exportReference = ResolveImportObjectReference(import, objPackage);
        if (exportReference.IsNull())
        {
            return;
        }

        if (exportReference.NativeClass is null && !_adj.ContainsKey(exportReference))
        {
            objQueue.Enqueue(exportReference);
        }

        AddEdge(exportReference, currentObj);
    }

    private PackageObjectReference ResolveImportClassReference(ImportTableItem import, UnrealPackage objPackage)
    {
        var fullName = objPackage.GetFullName(import);
        if (_fullNameToReferenceCache.ContainsKey(fullName))
        {
            CacheHits++;
            return _fullNameToReferenceCache[fullName];
        }

        CacheMisses++;

        var clsPackageName = objPackage.GetName(import.ClassPackage);
        var className = objPackage.GetName(import.ClassName);


        var importPackage = clsPackageName == objPackage.PackageName ? objPackage : _packagePackageCache.ResolveExportPackage(clsPackageName);
        if (importPackage is null)
        {
            //Debugger.Break();
            // null reference
            return new PackageObjectReference();
        }


        for (var index = 0; index < importPackage.ExportTable.Count; index++)
        {
            var exportTableItem = importPackage.ExportTable[index];
            if (exportTableItem.ClassIndex.Index != 0 || importPackage.GetName(exportTableItem.ObjectName) != className)
            {
                continue;
            }

            var exportIndex = new ObjectIndex(ObjectIndex.FromExportIndex(index));
            var resolveImportClassReference = new PackageObjectReference(clsPackageName, exportIndex);
            _fullNameToReferenceCache.Add(fullName, resolveImportClassReference);
            return resolveImportClassReference;
        }

        for (var index = 0; index < importPackage.ImportTable.Count; index++)
        {
            var x = importPackage.ImportTable[index];
            if (importPackage.GetName(x.ObjectName) != className)
            {
                continue;
            }

            var importIndex = new ObjectIndex(ObjectIndex.FromImportIndex(index));
            var resolveImportClassReference = new PackageObjectReference(clsPackageName, importIndex);
            _fullNameToReferenceCache.Add(fullName, resolveImportClassReference);
            return resolveImportClassReference;
        }

        var nativeClass = importPackage.FindClass(className);
        if (nativeClass is not null)
        {
            var resolveImportClassReference = new PackageObjectReference(clsPackageName, nativeClass);
            _fullNameToReferenceCache.Add(fullName, resolveImportClassReference);
            return resolveImportClassReference;
        }


        var packageObjectReference = new PackageObjectReference();
        _fullNameToReferenceCache.Add(fullName, packageObjectReference);
        //Debugger.Break();
        return packageObjectReference;
    }

    private void AddImportDependencies(ExportTableItem export, PackageObjectReference currentObj, Queue<PackageObjectReference> objQueue)
    {
        if (export.OuterIndex.Index != 0)
        {
            var outerReference = new PackageObjectReference(currentObj.PackageName, export.OuterIndex);
            if (!_adj.ContainsKey(outerReference))
            {
                objQueue.Enqueue(outerReference);
            }

            AddEdge(outerReference, currentObj);
        }

        if (export.ClassIndex.Index != 0)
        {
            var classReference = new PackageObjectReference(currentObj.PackageName, export.ClassIndex);
            if (!_adj.ContainsKey(classReference))
            {
                objQueue.Enqueue(classReference);
            }

            AddEdge(classReference, currentObj);
        }

        if (export.SuperIndex.Index != 0)
        {
            var supereReference = new PackageObjectReference(currentObj.PackageName, export.SuperIndex);
            if (!_adj.ContainsKey(supereReference))
            {
                objQueue.Enqueue(supereReference);
            }

            AddEdge(supereReference, currentObj);
        }

        if (export.ArchetypeIndex.Index != 0)
        {
            var archetypeReference = new PackageObjectReference(currentObj.PackageName, export.ArchetypeIndex);
            if (!_adj.ContainsKey(archetypeReference))
            {
                objQueue.Enqueue(archetypeReference);
            }

            AddEdge(archetypeReference, currentObj);
        }
    }

    // A recursive function used by topologicalSort
    private void TopologicalSortUtil(PackageObjectReference v, ISet<PackageObjectReference> visited,
        Stack<PackageObjectReference> stack)
    {
        // Mark the current node as visited.
        visited.Add(v);

        // Recur for all the vertices
        // adjacent to this vertex
        foreach (var vertex in _adj[v])
        {
            if (!visited.Contains(vertex.Dest))
            {
                TopologicalSortUtil(vertex.Dest, visited, stack);
            }
        }

        // Push current vertex to
        // stack which stores result
        stack.Push(v);
    }

    /// <summary>
    ///     https://www.geeksforgeeks.org/topological-sorting/
    ///     The function to do Topological Sort.
    ///     It uses recursive topologicalSortUtil()
    ///     MVN: I slightly modified the implementation to be compatible with my use case
    /// </summary>
    /// <returns></returns>
    public List<PackageObjectReference> TopologicalSort()
    {
        Stack<PackageObjectReference> stack = new();

        // Mark all the vertices as not visited
        var visited = new HashSet<PackageObjectReference>();

        // Call the recursive helper function
        // to store Topological Sort starting
        // from all vertices one by one
        foreach (var (key, _) in _adj)
        {
            if (!visited.Contains(key))
            {
                TopologicalSortUtil(key, visited, stack);
            }
        }

        return stack.ToList();
    }

    /// <summary>
    ///     Returns the fullname of a object reference. For debugger use, it's not safe to use in production
    /// </summary>
    /// <param name="objectReference"></param>
    /// <returns></returns>
    public string GetReferenceFullName(PackageObjectReference objectReference)
    {
        if (objectReference.NativeClass is not null)
        {
            return
                $"{objectReference.PackageName} : ({objectReference.NativeClass.Class!.Name}) {objectReference.PackageName}.{objectReference.NativeClass.Name}";
        }

        var importPackage = _packagePackageCache.ResolveExportPackage(objectReference.PackageName);
        ArgumentNullException.ThrowIfNull(importPackage);
        var obj = importPackage.GetObjectReference(objectReference.ObjectIndex);
        string? objTypeName;
        switch (obj)
        {
            case ExportTableItem exportTableItem:
                if (exportTableItem.ClassIndex.Index == 0)
                {
                    objTypeName = "Class";
                }
                else
                {
                    var objectResource = importPackage.GetObjectReference(exportTableItem.ClassIndex);
                    objTypeName = importPackage.GetName(objectResource!.ObjectName);
                }

                break;
            case ImportTableItem importTableItem:
                objTypeName = importPackage.GetName(importTableItem.ClassName);
                break;
            default:
                objTypeName = "unknown";
                break;
        }

        return obj == null ? "null" : $"{objectReference.PackageName}: ({objTypeName}) {importPackage.GetFullName(obj)}";
    }


    /// <summary>
    ///     Add a Edge representing a dependency between from and to. If any of the nodes are new. Add them
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void AddEdge(PackageObjectReference from, PackageObjectReference to)
    {
        if (from.Equals(to))
        {
            throw new ArgumentException("Can't create a edge between the same two nodes");
        }

        if (!_adj.ContainsKey(from))
        {
            AddNode(from);
        }

        if (!_adj.ContainsKey(to))
        {
            AddNode(to);
        }

        _adj[from].Add(new Edge(to));
    }

    /// <summary>
    ///     Get the edges from a node. This represents the objects that depends on this node. Throws if the node is not
    ///     registered to the graph
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public List<PackageObjectReference> GetEdges(PackageObjectReference node)
    {
        return _adj[node].Select(x => x.Dest).ToList();
    }

    internal readonly struct Edge : IEquatable<Edge>
    {
        public readonly PackageObjectReference Dest;

        public Edge(PackageObjectReference dest)
        {
            Dest = dest;
        }

        public bool Equals(Edge other)
        {
            return Dest.Equals(other.Dest);
        }

        public override bool Equals(object? obj)
        {
            return obj is Edge other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Dest.GetHashCode();
        }
    }
}