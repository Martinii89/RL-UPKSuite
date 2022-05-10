using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization.Default;
using Decryptor.Wpf.MVVM.Model;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Decryptor.Wpf.MVVM.ViewModel;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Unpacked");

    [ObservableProperty]
    private string _statusText = "Ready";

    private BackgroundWorker? _unpackBackgroundWorker;

    [ObservableProperty]
    private double _unpackProgress;

    public ICollectionView FilesAddedView;

    public MainWindowViewModel()
    {
        FilesAddedView = CollectionViewSource.GetDefaultView(FilesAdded);
        FilesAddedView.GroupDescriptions.Add(new PropertyGroupDescription("UnpackResult"));
    }

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
    public void OpenFolderSelectionDialog()
    {
        using var folderSelectDialog = new FolderBrowserDialog();
        folderSelectDialog.InitialDirectory = OutputDirectory;
        if (folderSelectDialog.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(folderSelectDialog.SelectedPath))
        {
            return;
        }

        OutputDirectory = folderSelectDialog.SelectedPath;
    }

    [ICommand]
    public void RemoveSelected(IList<object> selectedItems)
    {
        var fileReferences = selectedItems.Cast<FileReference>().ToList();
        foreach (var fileReference in fileReferences)
        {
            FilesAdded.Remove(fileReference);
        }

        DecryptFilesCommand.NotifyCanExecuteChanged();
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


            FilesAdded.Add(new FileReference { FilePath = file });
        }

        DecryptFilesCommand.NotifyCanExecuteChanged();
    }

    private bool CanStartUnpacking()
    {
        if (_unpackBackgroundWorker is { IsBusy: true })
        {
            return false;
        }

        return FilesAdded.Count != 0;
    }

    [ICommand(CanExecute = nameof(CanStartUnpacking))]
    private void DecryptFiles()
    {
        _unpackBackgroundWorker = new BackgroundWorker { WorkerReportsProgress = true };
        _unpackBackgroundWorker.DoWork += WorkerUnpackFiles;
        _unpackBackgroundWorker.ProgressChanged += WorkerProgressChanged;
        _unpackBackgroundWorker.RunWorkerCompleted += WorkerWorkCompleted;
        _unpackBackgroundWorker.RunWorkerAsync();
        DecryptFilesCommand.NotifyCanExecuteChanged();
    }

    private void WorkerUnpackFiles(object? sender, DoWorkEventArgs doWorkEventArgs)
    {
        if (sender is not BackgroundWorker worker)
        {
            throw new NullReferenceException("sender was not a background worker");
        }

        var decryptionProvider = new DecryptionProvider("keys.txt");
        double filesToProcess = FilesAdded.Count;
        var filesProcessed = 0;
        Parallel.ForEach(FilesAdded, new ParallelOptions { MaxDegreeOfParallelism = -1 }, fileReference =>
        {
            var inputFileName = Path.GetFileNameWithoutExtension(fileReference.FilePath);
            var outputFilePath = Path.Combine(OutputDirectory, inputFileName + "_decrypted.upk");
            var directoryInfo = new FileInfo(outputFilePath).Directory;
            Debug.Assert(directoryInfo != null);
            directoryInfo.Create();
            using var fileStream = File.Open(fileReference.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var decryptedStream = File.OpenWrite(outputFilePath);
            var unpacked = new RLPackageUnpacker(fileStream, decryptionProvider, FileSummarySerializer.GetDefaultSerializer());
            unpacked.Unpack(decryptedStream);
            fileReference.UnpackResult = unpacked.UnpackResult switch
            {
                UnpackResult.Success => "Success",
                _ => "Fail"
            };
            var counter = Interlocked.Increment(ref filesProcessed);
            worker.ReportProgress((int) (counter / filesToProcess * 100), fileReference);
        });


        //Triggers the WorkComplete event (I think?)
        doWorkEventArgs.Result = null;
    }

    private void WorkerProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        UnpackProgress = e.ProgressPercentage;
        StatusText = $"Unpacking: {e.ProgressPercentage}%";
        //var fileRef = e.UserState as FileReference;
        //if (fileRef != null)
        //{
        //    fileRef.
        //}
    }

    private void WorkerWorkCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        UnpackProgress = 0;
        StatusText = "Done";
        DecryptFilesCommand.NotifyCanExecuteChanged();
        FilesAddedView.Refresh();
        Process.Start("explorer.exe", OutputDirectory);
    }
}