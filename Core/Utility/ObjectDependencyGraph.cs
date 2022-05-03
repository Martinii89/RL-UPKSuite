using System.Text;
using Core.Types.PackageTables;

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

public class ObjectDependencyGraph
{
    private readonly Dictionary<int, Node> _nodes = new();

    public void AddExport(ExportTableItem exportTableItem, int index)
    {
        var outer = exportTableItem.OuterIndex.Index;
        if (outer != 0)
        {
            AddDependancyEdge(outer, index, EdgeType.Outer);
        }


        var super = exportTableItem.SuperIndex.Index;
        if (super != 0)
        {
            AddDependancyEdge(super, index, EdgeType.Super);
        }

        var archetype = exportTableItem.ArchetypeIndex.Index;
        if (archetype != 0)
        {
            AddDependancyEdge(archetype, index, EdgeType.Archetype);
        }

        var @class = exportTableItem.ClassIndex.Index;
        if (@class != 0)
        {
            AddDependancyEdge(@class, index, EdgeType.Class);
        }
    }

    internal Node GetOrAddNode(int key)
    {
        if (_nodes.TryGetValue(key, out var node))
        {
            return node;
        }

        var newNode = new Node(key);
        _nodes.Add(key, newNode);
        return newNode;
    }

    internal void AddDependancyEdge(int super, int @base, EdgeType dependancyType)
    {
        // TODO: Use edgetype
        var superNode = GetOrAddNode(super);
        var baseNode = GetOrAddNode(@base);
        superNode.OutgoingEdges.Add(baseNode);
        baseNode.IncomingEdges.Add(superNode);
    }

    public string GetDotFile()
    {
        var sb = new StringBuilder();
        sb.AppendLine("digraph graphname {");
        foreach (var (key, node) in _nodes)
        {
            foreach (var edge in node.OutgoingEdges)
            {
                sb.AppendLine($"\t{key} -> {edge.ObjectIndex};");
            }
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    internal enum EdgeType
    {
        Outer,
        Super,
        Archetype,
        Class
    }
}