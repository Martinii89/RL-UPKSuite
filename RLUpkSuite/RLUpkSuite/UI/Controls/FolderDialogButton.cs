using System.Windows;
using System.Windows.Controls;

using Microsoft.Win32;

namespace RlUpk.RLUpkSuite.UI.Controls;

public class FolderDialogButton : Button
{
    public static readonly DependencyProperty DialogTitleProperty = DependencyProperty.Register(
        nameof(DialogTitle), typeof(string), typeof(FolderDialogButton), new PropertyMetadata(default(string)));


    public static readonly DependencyProperty FolderProperty = DependencyProperty.Register(
        nameof(Folder), typeof(string), typeof(FolderDialogButton),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    static FolderDialogButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderDialogButton),
            new FrameworkPropertyMetadata(typeof(FolderDialogButton)));
    }

    public FolderDialogButton()
    {
        Click += OnClick;
    }

    public string Folder
    {
        get => (string)GetValue(FolderProperty);
        set => SetValue(FolderProperty, value);
    }

    public string DialogTitle
    {
        get => (string)GetValue(DialogTitleProperty);
        set => SetValue(DialogTitleProperty, value);
    }


    private void OnClick(object sender, RoutedEventArgs e)
    {
        OpenFolderDialog folderDialog = new()
        {
            Title = DialogTitle
        };
        bool? success = folderDialog.ShowDialog();
        if (success != true)
        {
            return;
        }

        string folder = folderDialog.FolderName;
        Folder = folder;
    }
}