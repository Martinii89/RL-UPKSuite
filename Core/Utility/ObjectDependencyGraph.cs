namespace Core.Utility;

internal class Node : IEquatable<Node>
{
    internal Node(int objectIndex)
    {
        ObjectIndex = objectIndex;
    }

    internal List<Node> OutgoingEdges { get; set; } = new();
    internal List<Node> IncomingEdges { get; set; } = new();

    internal int ObjectIndex { get; }

    public bool Equals(Node? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ObjectIndex == other.ObjectIndex;
    }

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

        return Equals((Node) obj);
    }

    public override int GetHashCode()
    {
        return ObjectIndex;
    }
}

internal class Edge
{
    public Edge(int dest, EdgeType type)
    {
        Dest = dest;
        Type = type;
    }

    public int Dest { get; set; }
    public EdgeType Type { get; set; }
}

public enum EdgeType
{
    Outer,
    Super,
    Archetype,
    Class
}

public class Graph
{
    private readonly Dictionary<int, List<Edge>> _adj = new();
    private int _nodeCount;

    public void AddEdge(int from, int to, EdgeType edgeType)
    {
        var adjList = GetOrAddNodeEdges(from);
        GetOrAddNodeEdges(to);
        adjList.Add(new Edge(to, edgeType));
    }

    // A recursive function used by topologicalSort
    private void TopologicalSortUtil(int v, HashSet<int> visited,
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
        foreach (var (key, value) in _adj)
        {
            if (!visited.Contains(key))
            {
                TopologicalSortUtil(key, visited, stack);
            }
        }

        return stack.ToList();
    }

    private List<Edge> GetOrAddNodeEdges(int from)
    {
        if (_adj.TryGetValue(from, out var adjList))
        {
            return adjList;
        }

        _nodeCount++;
        adjList = new List<Edge>();
        _adj.Add(from, adjList);
        return adjList;
    }
}