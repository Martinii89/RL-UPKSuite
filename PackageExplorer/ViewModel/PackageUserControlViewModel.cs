using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Types;
using PackageExplorer.Model;
using Syncfusion.UI.Xaml.TreeView.Engine;

namespace PackageExplorer.ViewModel;

public partial class PackageUserControlViewModel : ObservableObject
{
    private readonly UnrealPackage _package;

    [ObservableProperty]
    private PackageObject? _selectedNode;

    public PackageUserControlViewModel(UnrealPackage package)
    {
        _package = package;
        foreach (var export in package.ExportTable.Where(x => x.OuterIndex.Index == 0))
        {
            var exportedObject = export.Object;

            if (exportedObject is null || exportedObject.Class?.Name == "Class")
            {
                continue;
            }

            var obj = new PackageObject(exportedObject);
            AddTopLevelObject(obj);
        }
    }

    public string PackageName => _package.PackageName;

    public ObservableCollection<PackageObject> TopLevelObjects { get; set; } = new();

    [ICommand(CanExecute = nameof(CanExecuteOnDemandLoading))]
    private void ExecuteOnDemandLoading(TreeViewNode node)
    {
        //if (obj is not TreeViewNode node)
        //{
        //    return;
        //}

        if (node.ChildNodes.Count > 0)
        {
            node.IsExpanded = true;
            return;
        }

        if (node.Content is not PackageObject packageObject)
        {
            return;
        }

        //PopulateChildren(packageObject);

        if (packageObject.Children.Count == 0)
        {
            node.HasChildNodes = false;
            return;
        }

        foreach (var child in packageObject.Children)
        {
            PopulateChildren(child);
        }

        node.PopulateChildNodes(packageObject.Children);
        //foreach (var childNode in node.ChildNodes)
        //{
        //    if (childNode.Content is not PackageObject subObj)
        //    {
        //        continue;
        //    }

        //    PopulateChildren(subObj);
        //    if (subObj.Children.Count == 0)
        //    {
        //        childNode.HasChildNodes = false;
        //    }
        //}

        node.IsExpanded = true;
    }

    public void AddTopLevelObject(PackageObject obj)
    {
        PopulateChildren(obj);
        TopLevelObjects.Add(obj);
    }

    private bool CanExecuteOnDemandLoading(TreeViewNode node)
    {
        return (node.Content as PackageObject).HasSubObjects;
    }

    private void PopulateChildren(PackageObject packageObject)
    {
        foreach (var subObject in _package.ExportTable.Where(x => x.Object?.Outer == packageObject.Object))
        {
            packageObject.Children.Add(new PackageObject(subObject.Object!));
        }
    }
}