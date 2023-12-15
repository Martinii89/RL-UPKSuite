using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization.Default;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using RLUpkSuite.Config;
using RLUpkSuite.Pages;

namespace RLUpkSuite.ViewModels;

public partial class DecryptorPageViewModel : PageBase
{
    private readonly CommonConfig _commonConfig;

    private readonly DecryptionConfig _decryptionConfig;

    private readonly IDecrypterProvider _decryptionProvider;

    private readonly IMessenger _messenger;

    public DecryptorPageViewModel(
        DecryptionConfig decryptionConfig,
        CommonConfig commonConfig,
        IMessenger messenger,
        IDecrypterProvider decryptionProvider)
        : base("Decryption", PackIconKind.ArchiveArrowUp)
    {
        _decryptionConfig = decryptionConfig;
        _commonConfig = commonConfig;
        _messenger = messenger;
        _decryptionProvider = decryptionProvider;
    }


    public bool OpenOutputDirectoryOnFinished
    {
        get => _decryptionConfig.ShowOutputDirectory;
        set => SetProperty(_decryptionConfig.ShowOutputDirectory, value, _decryptionConfig,
            (config, b) => config.ShowOutputDirectory = b);
    }

    public string? KeyPath
    {
        get => _commonConfig.KeysPath;
        set
        {
            if (SetProperty(_commonConfig.KeysPath, value, _commonConfig, (config, s) => config.KeysPath = s))
                DecryptFilesCommand.NotifyCanExecuteChanged();
        }
    }

    public string? OutputDirectory
    {
        get => _decryptionConfig.OutputDirectory;
        set
        {
            if (SetProperty(_decryptionConfig.OutputDirectory, value, _decryptionConfig,
                    (config, s) => config.OutputDirectory = s)) DecryptFilesCommand.NotifyCanExecuteChanged();
        }
    }

    public ObservableCollection<FileReference> FileReferences { get; set; } = [];

    private async Task AddFiles(IEnumerable<string> paths)
    {
        List<string> validFiles = [];
        await Task.Run(() => { validFiles.AddRange(paths.Where(Path.Exists)); });
        foreach (var validFile in validFiles) FileReferences.Add(new FileReference(validFile));
    }


    [RelayCommand]
    private async Task OpenFileDialog()
    {
        OpenFileDialog fileDialog = new()
        {
            Title = "Select UPK files", Multiselect = true, Filter = "Upk files | *.upk"
        };
        var success = fileDialog.ShowDialog();
        if (success != true) return;

        string[] files = fileDialog.FileNames;
        await AddFiles(files);
    }

    [RelayCommand(CanExecute = nameof(CanDecryptFiles))]
    private async Task DecryptFiles()
    {
        await Task.Run(ProcessFiles);
        if (OutputDirectory is not null && OpenOutputDirectoryOnFinished)
            Process.Start("explorer.exe", OutputDirectory);
    }

    private bool CanDecryptFiles()
    {
        return !string.IsNullOrEmpty(OutputDirectory) && !string.IsNullOrEmpty(KeyPath);
    }

    private void ProcessFiles()
    {
        if (string.IsNullOrEmpty(OutputDirectory) || string.IsNullOrEmpty(KeyPath)) return;

        var filesProcessed = 0;
        _decryptionProvider.UseKeyFile(KeyPath);
        Parallel.ForEach(FileReferences, new ParallelOptions
        {
            MaxDegreeOfParallelism = -1
        }, fileReference =>
        {
            if (fileReference.ProcessSuccess)
            {
                Interlocked.Increment(ref filesProcessed);
                return;
            }

            var inputFileName = Path.GetFileNameWithoutExtension(fileReference.FilePath);
            var outputFilePath = Path.Combine(OutputDirectory, inputFileName + "_decrypted.upk");
            var directoryInfo = new FileInfo(outputFilePath).Directory;
            Debug.Assert(directoryInfo != null);
            directoryInfo.Create();
            using var fileStream = File.Open(fileReference.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var decryptedStream = File.OpenWrite(outputFilePath);
            RLPackageUnpacker unpacked = new(fileStream, _decryptionProvider,
                FileSummarySerializer.GetDefaultSerializer());
            unpacked.Unpack(decryptedStream);
            fileReference.ProcessSuccess = unpacked.UnpackResult == UnpackResult.Success;
            Interlocked.Increment(ref filesProcessed);
        });
    }
}