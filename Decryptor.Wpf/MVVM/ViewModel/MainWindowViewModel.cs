using System;
using System.Collections.ObjectModel;
using System.IO;
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
    public void RemoveFile(FileReference fileReference)
    {
        FilesAdded.Remove(fileReference);
    }

    [ICommand]
    public void AddFiles(string[] files)
    {
        var validFiles = files.Where(file => Path.GetExtension(file) == ".upk"); 
        foreach (var file in validFiles)
        {

            if (!File.Exists(file))
            {
                MessageBox.Show("Error: File does not exist");
            }
            

            FilesAdded.Add(new FileReference() { FilePath = file });
        }
    }

    [ICommand]
    private void DecryptFiles(IEnumerable<FileReference> files)
    {

    }
}