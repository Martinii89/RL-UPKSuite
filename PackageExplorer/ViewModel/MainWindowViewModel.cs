using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Types;
using Microsoft.Win32;
using PackageExplorer.Utils;

namespace PackageExplorer.ViewModel;

public partial class MainWindowViewModel : ObservableObject
{
    private BackgroundWorker? _unpackBackgroundWorker;

    public ObservableCollection<PackageUserControlViewModel> LoadedPackages { get; set; } = new();

    [RelayCommand]
    private void OpenFileDialog()
    {
        var openFileDialog = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "Package files (*.upk;*.u)|*.upk;*.u"
        };
        if (openFileDialog.ShowDialog() != true)
        {
            return;
        }

        OpenPackageFile(openFileDialog.FileName);

        //MessageBox.Show($"Open: {string.Join(",", openFileDialog.FileNames)}");
    }

    private void OpenPackageFile(string packageFilePath)
    {
        _unpackBackgroundWorker = new BackgroundWorker { WorkerReportsProgress = false };
        _unpackBackgroundWorker.DoWork += (sender, args) =>
        {
            var folderPath = Path.GetDirectoryName(packageFilePath) ?? string.Empty;
            var searchPaths = new List<string>
            {
                folderPath
            };
            var packageName = Path.GetFileNameWithoutExtension(packageFilePath);
            var package = PackageLoader.LoadPackage(packageFilePath, packageName, searchPaths, null);
            if (package == null)
            {
                return;
            }

            args.Result = package;
        };
        _unpackBackgroundWorker.RunWorkerCompleted += WorkerWorkCompleted;
        _unpackBackgroundWorker.RunWorkerAsync();
    }

    private void WorkerWorkCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        var package = e.Result as UnrealPackage;
        if (package == null)
        {
            return;
        }

        var vm = new PackageUserControlViewModel(package);
        LoadedPackages.Add(vm);
    }
}