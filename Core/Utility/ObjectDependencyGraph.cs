using Core.Types.PackageTables;

namespace Core.Utility;

internal readonly struct Edge
{
    public Edge(int dest)
    {
        Dest = dest;
    }

    public int Dest { get; }
}

/// <summary>
///     Super simple graph class. Used to construct a dependency graph that we can do a topological sort on. This gives us
///     a good object initialization order.
/// </summary>
public class ObjectDependencyGraph
{
    private readonly Dictionary<int, List<Edge>> _adj = new();

    /// <summary>
    ///     Add a edge to the dependency graph indicating that to depends on from.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void AddEdge(int from, int to)
    {
        var adjList = GetNodeEdges(from);
        GetNodeEdges(to);
        adjList.Add(new Edge(to));
    }

    /// <summary>
    ///     Adds the dependencies for the whole <see cref="ImportTable" />
    /// </summary>
    /// <param name="importTable"></param>
    public void AddImportTableDependencies(ImportTable importTable)
    {
        for (var index = 0; index < importTable.Count; index++)
        {
            var import = importTable[index];
            var outerIndex = import.OuterIndex.Index;
            if (outerIndex != 0)
            {
                AddEdge(outerIndex, ObjectIndex.FromImportIndex(index));
            }
        }
    }

    /// <summary>
    ///     Adds the dependencies for the whole <see cref="ExportTable" />
    /// </summary>
    /// <param name="exportTable"></param>
    public void AddExportTableDependencies(ExportTable exportTable)
    {
        // Add exports to dependency graph
        for (var index = 0; index < exportTable.Count; index++)
        {
            var export = exportTable[index];
            var outer = export.OuterIndex.Index;
            if (outer != 0)
            {
                AddEdge(outer, ObjectIndex.FromExportIndex(index));
            }

            var super = export.SuperIndex.Index;
            if (super != 0)
            {
                AddEdge(super, ObjectIndex.FromExportIndex(index));
            }

            var archetype = export.ArchetypeIndex.Index;
            if (archetype != 0)
            {
                AddEdge(archetype, ObjectIndex.FromExportIndex(index));
            }

            var @class = export.ClassIndex.Index;
            if (@class != 0)
            {
                AddEdge(@class, ObjectIndex.FromExportIndex(index));
            }
        }
    }

    // A recursive function used by topologicalSort
    private void TopologicalSortUtil(int v, ISet<int> visited,
        Stack<int> stack)
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
    public List<int> TopologicalSort()
    {
        Stack<int> stack = new();

        // Mark all the vertices as not visited
        var visited = new HashSet<int>();

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

    private List<Edge> GetNodeEdges(int nodeId)
    {
        if (_adj.TryGetValue(nodeId, out var adjList))
        {
            return adjList;
        }

        adjList = new List<Edge>();
        _adj.Add(nodeId, adjList);
        return adjList;
    }
}