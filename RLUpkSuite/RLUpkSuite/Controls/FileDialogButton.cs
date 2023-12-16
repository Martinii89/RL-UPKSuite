using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Win32;

namespace RLUpkSuite.Controls;

public class FileDialogButton : Button
{
    public static readonly DependencyProperty FileFilterProperty = DependencyProperty.Register(
        nameof(FileFilter), typeof(string), typeof(FileDialogButton), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty FileSelectedCommandProperty = DependencyProperty.Register(
        nameof(FileSelectedCommand), typeof(ICommand), typeof(FileDialogButton),
        new PropertyMetadata(default(ICommand)));

    public static readonly DependencyProperty DialogTitleProperty = DependencyProperty.Register(
        nameof(DialogTitle), typeof(string), typeof(FileDialogButton), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty MultiSelectProperty = DependencyProperty.Register(
        nameof(MultiSelect), typeof(bool), typeof(FileDialogButton), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty FileProperty = DependencyProperty.Register(
        nameof(File), typeof(string), typeof(FileDialogButton),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty FilesProperty = DependencyProperty.Register(
        nameof(Files), typeof(IEnumerable<string>), typeof(FileDialogButton),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public IEnumerable<string> Files
    {
        get { return (IEnumerable<string>)GetValue(FilesProperty); }
        set { SetValue(FilesProperty, value); }
    }

    public string File
    {
        get { return (string)GetValue(FileProperty); }
        set { SetValue(FileProperty, value); }
    }

    public string FileFilter
    {
        get => (string)GetValue(FileFilterProperty);
        set => SetValue(FileFilterProperty, value);
    }

    public ICommand FileSelectedCommand
    {
        get => (ICommand)GetValue(FileSelectedCommandProperty);
        set => SetValue(FileSelectedCommandProperty, value);
    }

    public string DialogTitle
    {
        get => (string)GetValue(DialogTitleProperty);
        set => SetValue(DialogTitleProperty, value);
    }

    public bool MultiSelect
    {
        get => (bool)GetValue(MultiSelectProperty);
        set => SetValue(MultiSelectProperty, value);
    }

    static FileDialogButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(FileDialogButton),
            new FrameworkPropertyMetadata(typeof(FileDialogButton)));
    }

    public FileDialogButton()
    {
        Click += OnClick;
    }


    private void OnClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog fileDialog = new()
        {
            Title = DialogTitle, Multiselect = MultiSelect, Filter = FileFilter
        };
        bool? success = fileDialog.ShowDialog();
        if (success != true)
        {
            return;
        }

        if (MultiSelect)
        {
            string[] files = fileDialog.FileNames;
            Files = files;
            FileSelectedCommand?.Execute(files);
        }
        else
        {
            string file = fileDialog.FileName;
            // SetValue(FileProperty, file);
            File = file;
            FileSelectedCommand?.Execute(file);
        }
    }
}