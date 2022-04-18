using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Decryptor.Wpf.MVVM.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;

namespace Decryptor.Wpf.MVVM.ViewModel;

public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<FileReference> FilesAdded { get; } = new();

    [ICommand]
    public void OpenFileDialog()
    {
        var openFileDialog = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "UPK files (*.upk)|*.upk"
        };
        if (openFileDialog.ShowDialog() != true)
        {
            return;
        }

        AddFiles(openFileDialog.FileNames);
    }

    [ICommand]
    public void AddFiles(string[] files)
    {
        foreach (var file in files)
        {
            if (!File.Exists(file))
            {
                MessageBox.Show("Error: File does not exist");
            }

            FilesAdded.Add(new FileReference() { FilePath = file });
        }
    }
}