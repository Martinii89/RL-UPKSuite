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
    private readonly DecryptionConfig _decryptionConfig;

    private readonly IDecrypterProvider _decryptionProvider;

    public DecryptorPageViewModel(
        DecryptionConfig decryptionConfig,
        IDecrypterProvider decryptionProvider)
        : base("Decryption", PackIconKind.ArchiveArrowUp)
    {
        _decryptionConfig = decryptionConfig;
        _decryptionProvider = decryptionProvider;
    }


    public bool OpenOutputDirectoryOnFinished
    {
        get => _decryptionConfig.OpenOutputOnFinish;
        set => SetProperty(_decryptionConfig.OpenOutputOnFinish, value, _decryptionConfig,
            (config, b) => config.OpenOutputOnFinish = b);
    }

    public string? KeyPath
    {
        get => _decryptionConfig.KeysPath;
        set
        {
            if (SetProperty(_decryptionConfig.KeysPath, value, _decryptionConfig, (config, s) => config.KeysPath = s))
            {
                DecryptFilesCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string? OutputDirectory
    {
        get => _decryptionConfig.OutputDirectory;
        set
        {
            if (SetProperty(_decryptionConfig.OutputDirectory, value, _decryptionConfig,
                    (config, s) => config.OutputDirectory = s))
            {
                DecryptFilesCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public FileReferenceCollection FileReferences { get; set; } = [];

    [RelayCommand]
    private async Task AddFiles(IEnumerable<string> paths)
    {
        List<string> validFiles = [];
        await Task.Run(() => { validFiles.AddRange(paths.Where(Path.Exists)); });
        foreach (string validFile in validFiles)
        {
            FileReferences.Add(new FileReference(validFile));
        }
    }
    

    [RelayCommand(CanExecute = nameof(CanDecryptFiles))]
    private async Task DecryptFiles()
    {
        await Task.Run(ProcessFiles);
        if (OutputDirectory is not null && OpenOutputDirectoryOnFinished)
        {
            Process.Start("explorer.exe", OutputDirectory);
        }
    }

    private bool CanDecryptFiles()
    {
        return !string.IsNullOrEmpty(OutputDirectory) && !string.IsNullOrEmpty(KeyPath);
    }

    private void ProcessFiles()
    {
        if (string.IsNullOrEmpty(OutputDirectory) || string.IsNullOrEmpty(KeyPath))
        {
            return;
        }

        int filesProcessed = 0;
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

            string inputFileName = Path.GetFileNameWithoutExtension(fileReference.FilePath);
            string outputFilePath = Path.Combine(OutputDirectory, inputFileName + "_decrypted.upk");
            DirectoryInfo? directoryInfo = new FileInfo(outputFilePath).Directory;
            Debug.Assert(directoryInfo != null);
            directoryInfo.Create();
            using FileStream fileStream =
                File.Open(fileReference.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using FileStream decryptedStream = File.OpenWrite(outputFilePath);
            RLPackageUnpacker unpacked = new(fileStream, _decryptionProvider,
                FileSummarySerializer.GetDefaultSerializer());
            unpacked.Unpack(decryptedStream);
            fileReference.ProcessSuccess = unpacked.UnpackResult == UnpackResult.Success;
            Interlocked.Increment(ref filesProcessed);
        });
    }
}