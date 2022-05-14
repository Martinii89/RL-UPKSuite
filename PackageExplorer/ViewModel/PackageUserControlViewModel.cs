using System.Collections.ObjectModel;
using PackageExplorer.Model;

namespace PackageExplorer.ViewModel;

public class PackageUserControlViewModel
{
    public PackageUserControlViewModel()
    {
        var obj1 = new PackageObject { Name = "Obj1", TypeName = "class1" };
        var obj2 = new PackageObject { Name = "Obj2", TypeName = "class2" };
        var obj3 = new PackageObject { Name = "Obj3", TypeName = "class3" };
        var obj4 = new PackageObject { Name = "Obj4", TypeName = "class4" };
        var obj5 = new PackageObject { Name = "Obj5", TypeName = "class5" };
        var obj6 = new PackageObject { Name = "Obj6", TypeName = "class5" };

        TopLevelObjects.Add(obj1);
        TopLevelObjects.Add(obj2);
        TopLevelObjects.Add(obj3);
        obj1.Children.Add(obj4);
        obj1.Children.Add(obj5);
        obj4.Children.Add(obj6);
        for (var i = 0; i < 100; i++)
        {
            TopLevelObjects.Add(new PackageObject { Name = $"obj{i}", TypeName = $"class{i}" });
        }
    }

    public string TestContent { get; set; } = "asss";

    public ObservableCollection<PackageObject> TopLevelObjects { get; set; } = new();
}