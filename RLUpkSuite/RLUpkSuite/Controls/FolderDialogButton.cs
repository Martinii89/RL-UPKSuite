using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Win32;

namespace RLUpkSuite.Controls;

public class FolderDialogButton : Button
{
    public static readonly DependencyProperty DialogTitleProperty = DependencyProperty.Register(
        nameof(DialogTitle), typeof(string), typeof(FolderDialogButton), new PropertyMetadata(default(string)));


    public static readonly DependencyProperty FolderProperty = DependencyProperty.Register(
        nameof(Folder), typeof(string), typeof(FolderDialogButton),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string Folder
    {
        get { return (string)GetValue(FolderProperty); }
        set { SetValue(FolderProperty, value); }
    }

    static FolderDialogButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderDialogButton),
            new FrameworkPropertyMetadata(typeof(FolderDialogButton)));
    }

    public FolderDialogButton()
    {
        Click += OnClick;
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