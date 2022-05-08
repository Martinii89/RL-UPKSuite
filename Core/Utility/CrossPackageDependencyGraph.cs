using Core.Classes.Core;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Utility;

/// <summary>
///     A PackageObjectReference points to a unique object in a package, or a native only class for self imported
///     classes.
/// </summary>
public class PackageObjectReference : IEquatable<PackageObjectReference>
{
    /// <summary>
    ///     This references a native only class in a specific package
    /// </summary>
    public readonly UClass? NativeClass;

    /// <summary>
    ///     A index representing either a import or a export object from a package
    /// </summary>
    public readonly ObjectIndex ObjectIndex;

    /// <summary>
    ///     The name of the package that the ObjectIndex originates from
    /// </summary>
    public readonly string PackageName;


    /// <summary>
    ///     References a unique object in a package
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="objectIndex"></param>
    public PackageObjectReference(string packageName, ObjectIndex objectIndex)
    {
        ObjectIndex = objectIndex;
        PackageName = packageName;
        NativeClass = null;
    }

    /// <summary>
    ///     A package reference for a native only class object with no export\import table entry
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="nativeClass"></param>
    public PackageObjectReference(string packageName, UClass nativeClass)
    {
        ObjectIndex = new ObjectIndex();
        PackageName = packageName;
        NativeClass = nativeClass;
    }

    /// <inheritdoc />
    public bool Equals(PackageObjectReference? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Equals(NativeClass, other.NativeClass) && ObjectIndex.Equals(other.ObjectIndex) && PackageName == other.PackageName;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((PackageObjectReference) obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(NativeClass, ObjectIndex, PackageName);
    }
}

/// <summary>
///     Helper class to construct a complete dependency graph, including all required objects from imported packages
/// </summary>
public class CrossPackageDependencyGraph
{
    private readonly Dictionary<PackageObjectReference, HashSet<Edge>> _adj = new();

    private readonly IImportResolver _packageImportResolver;

    /// <summary>
    ///     Create a package dependency graph. A import resolver is required to resolve import object dependencies correctly.
    /// </summary>
    /// <param name="packageImportResolver"></param>
    public CrossPackageDependencyGraph(IImportResolver packageImportResolver)
    {
        _packageImportResolver = packageImportResolver;
    }

    /// <summary>
    ///     How many objects in the graph
    /// </summary>
    public int NodeCount => _adj.Count;


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
        var importPackage = _packageImportResolver.ResolveExportPackage(packageName);
        var fullName = objPackage.GetFullName(import);
        if (importPackage is null)
        {
            throw new InvalidOperationException($"Can't find the package to resolve dependencies for {fullName}");
        }

        var nameParts = fullName.Split('.');
        var exportFullNameMatch =
            importPackage.ExportTable.FirstOrDefault(x => importPackage.GetName(x.ObjectName) == nameParts[^1] && importPackage.GetFullName(x) == fullName);
        if (exportFullNameMatch is not null)
        {
            var exportIndex = new ObjectIndex(ObjectIndex.FromExportIndex(importPackage.ExportTable.IndexOf(exportFullNameMatch)));
            return new PackageObjectReference(packageName, exportIndex);
        }

        var importFullNameMatch =
            importPackage.ImportTable.FirstOrDefault(x => importPackage.GetName(x.ObjectName) == nameParts[^1] && importPackage.GetFullName(x) == fullName);
        if (importFullNameMatch is not null)
        {
            var importIndex = new ObjectIndex(ObjectIndex.FromImportIndex(importPackage.ImportTable.IndexOf(importFullNameMatch)));
            return new PackageObjectReference(packageName, importIndex);
        }

        var nativeClass = importPackage.FindClass(objPackage.GetName(import.ObjectName));
        if (nativeClass is not null)
        {
            return new PackageObjectReference(packageName, nativeClass);
        }

        throw new InvalidOperationException($"Failed to find the import object: {objPackage.GetFullName(import)}");
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
            var objPackage = _packageImportResolver.ResolveExportPackage(currentObj.PackageName);
            if (objPackage is null)
            {
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
        if (import.OuterIndex.Index != 0)
        {
            // Native imports has no export reference to resolve
            var outerReference = new PackageObjectReference(currentObj.PackageName, import.OuterIndex);
            if (!_adj.ContainsKey(outerReference))
            {
                objQueue.Enqueue(outerReference);
            }

            AddEdge(outerReference, currentObj);


            if (ImportIsNative(import, objPackage))
            {
                return;
            }

            var exportReference = ResolveImportObjectReference(import, objPackage);
            if (exportReference.NativeClass is null && !_adj.ContainsKey(exportReference))
            {
                objQueue.Enqueue(exportReference);
            }

            AddEdge(exportReference, currentObj);
        }
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

        var importPackage = _packageImportResolver.ResolveExportPackage(objectReference.PackageName);
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

    internal class Edge : IEquatable<Edge>
    {
        public readonly PackageObjectReference Dest;

        public Edge(PackageObjectReference dest)
        {
            Dest = dest;
        }

        /// <inheritdoc />
        public bool Equals(Edge? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Dest.Equals(other.Dest);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Edge) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Dest.GetHashCode();
        }
    }
}