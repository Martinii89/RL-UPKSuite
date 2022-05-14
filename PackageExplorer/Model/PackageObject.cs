using System.Collections.ObjectModel;

namespace PackageExplorer.Model;

public class PackageObject
{
    public string Name { get; set; }
    public string TypeName { get; set; }

    public ObservableCollection<PackageObject> Children { get; set; } = new();
}