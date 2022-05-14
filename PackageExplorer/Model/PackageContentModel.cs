using System.Collections.ObjectModel;

namespace PackageExplorer.Model;

public class PackageContentViewModel
{
    public ObservableCollection<PackageObject> TopLevelObjects { get; set; } = new();
}