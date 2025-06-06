﻿using System.Collections.ObjectModel;
using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;

namespace RlUpk.RLUpkSuite.ViewModels;

public partial class FileReference(string filePath) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FileName))]
    private string _filePath = filePath;

    [ObservableProperty]
    private bool _processed;

    [ObservableProperty]
    private bool _processSuccess;

    public string FileName => Path.GetFileName((string?)FilePath);
}

public class FileReferenceCollection : ObservableCollection<FileReference>
{
}