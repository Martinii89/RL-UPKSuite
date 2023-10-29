using System.Collections.ObjectModel;
using System.Text;
using Core.Classes.Core;

namespace PackageExplorer.Model;

public class PackageObject

{
    public PackageObject(UObject o)
    {
        Object = o;
    }

    public UObject Object { get; init; }

    public string FullName
    {
        get
        {
            var sb = new StringBuilder();
            sb.Append(Name);
            var outer = Object.Outer;
            while (outer is not null)
            {
                sb.Insert(0, ".");
                sb.Insert(0, outer.Name);
                outer = outer.Outer;
            }

            if (Object.Class is not null)
            {
                sb.Insert(0, $"({Object.Class.Name}) ");
            }

            return sb.ToString();
        }
    }

    public string ArchetypeName => Object.ObjectArchetype?.Name ?? "Null";

    public string Name => Object.Name;
    public string TypeName => Object.Class?.Name ?? "Null";

    public string NameWithType => $"{Name} ({TypeName})";

    public ObservableCollection<PackageObject> Children { get; set; } = new();

    public bool HasSubObjects => Children.Count > 0;


    public bool IsDefaultObject => Object.IsDefaultObject;

    public bool IsArchetypeObject => Object.IsArchetypeObject;
}