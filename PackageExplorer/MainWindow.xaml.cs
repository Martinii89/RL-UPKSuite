using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using PackageExplorer.ViewModel;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Shared;

namespace PackageExplorer;

public class MyObservableCollection : ObservableCollection<object>
{
}

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : ChromelessWindow
{
    public MainWindow()
    {
        SfSkinManager.ApplyStylesOnApplication = true;
        SfSkinManager.SetVisualStyle(this, VisualStyles.FluentDark);
        SfSkinManager.ApplyStylesOnApplication = false;
        InitializeComponent();

        DataContext = new MainWindowViewModel();
        //Loaded += OnLoaded;
        //var headeritems = LeftHeaderItemsSource.Cast<TabControlExt>().FirstOrDefault();
        //if (headeritems is not null)
        //{
        //    headeritems.Loaded += (sender, args) =>
        //    {
        //        var b1 = VisualUtils.FindDescendant(sender as TabControlExt, typeof(Border)) as Border;
        //        b1.Visibility = Visibility.Hidden;
        //    };
        //}
    }

    public static IEnumerable<T> FindVisualChilds<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj == null)
        {
            yield return (T) Enumerable.Empty<T>();
        }

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var ithChild = VisualTreeHelper.GetChild(depObj, i);
            if (ithChild == null)
            {
                continue;
            }

            if (ithChild is T t)
            {
                yield return t;
            }

            foreach (var childOfChild in FindVisualChilds<T>(ithChild))
            {
                yield return childOfChild;
            }
        }
    }

    ///// <summary>
    /////     Gets or sets the current visual style.
    ///// </summary>
    ///// <value></value>
    ///// <remarks></remarks>
    //public string CurrentVisualStyle
    //{
    //    get => _currentVisualStyle;
    //    set
    //    {
    //        _currentVisualStyle = value;
    //        OnVisualStyleChanged();
    //    }
    //}

    ///// <summary>
    /////     Gets or sets the current Size mode.
    ///// </summary>
    ///// <value></value>
    ///// <remarks></remarks>
    //public string CurrentSizeMode
    //{
    //    get => _currentSizeMode;
    //    set
    //    {
    //        _currentSizeMode = value;
    //        OnSizeModeChanged();
    //    }
    //}

    ///// <summary>
    /////     Called when [loaded].
    ///// </summary>
    ///// <param name="sender">The sender.</param>
    ///// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    //private void OnLoaded(object sender, RoutedEventArgs e)
    //{
    //    CurrentVisualStyle = "FluentDark";
    //    CurrentSizeMode = "Default";
    //}

    ///// <summary>
    /////     On Visual Style Changed.
    ///// </summary>
    ///// <remarks></remarks>
    //private void OnVisualStyleChanged()
    //{
    //    var parsed = Enum.TryParse(CurrentVisualStyle, out VisualStyles visualStyle);
    //    if (!parsed || visualStyle == VisualStyles.Default)
    //    {
    //        return;
    //    }

    //    SfSkinManager.ApplyStylesOnApplication = true;
    //    SfSkinManager.SetVisualStyle(this, visualStyle);
    //    SfSkinManager.ApplyStylesOnApplication = false;
    //}

    ///// <summary>
    /////     On Size Mode Changed event.
    ///// </summary>
    ///// <remarks></remarks>
    //private void OnSizeModeChanged()
    //{
    //    var parseResult = Enum.TryParse(CurrentSizeMode, out SizeMode sizeMode);
    //    if (!parseResult || sizeMode == SizeMode.Default)
    //    {
    //        return;
    //    }

    //    SfSkinManager.ApplyStylesOnApplication = true;
    //    SfSkinManager.SetSizeMode(this, sizeMode);
    //    SfSkinManager.ApplyStylesOnApplication = false;
    //}
}