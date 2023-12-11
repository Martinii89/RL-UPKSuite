using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Animation;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Core;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization.Default;

using MaterialDesignThemes.Wpf;

using Microsoft.Win32;

using RLUpkSuite.Pages;

namespace RLUpkSuite.ViewModels
{
    public partial class DecryptorPageViewModel : PageBase
    {
        private readonly IDecrypterProvider _decryptionProvider;
        private readonly UserConfiguration _userConfiguration;
        private readonly IMessenger _messenger;

        [ObservableProperty]
        private string _outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Unpacked");

        public DecryptorPageViewModel(
            UserConfiguration userConfiguration,
            IMessenger messenger, 
            IDecrypterProvider decryptionProvider) : base("Decryption", PackIconKind.ArchiveArrowUp)
        {
            _userConfiguration = userConfiguration;
            _messenger = messenger;
            _decryptionProvider = decryptionProvider;
        }

        public ObservableCollection<FileReference> FileReferences { get; set; } = [];

        private async Task AddFiles(IEnumerable<string> paths)
        {
            List<string> validFiles = [];
            await Task.Run(() =>
            {
                validFiles.AddRange(paths.Where(Path.Exists));
            });
            foreach (string validFile in validFiles)
            {
                FileReferences.Add(new FileReference(validFile));
            }
        }

        [RelayCommand]
        private void DeleteSelected(object args)
        {
            if (args is not IEnumerable selectedFiles)
            {
                return;
            }

            foreach (FileReference fileReference in selectedFiles.Cast<FileReference>().ToList())
            {
                FileReferences.Remove(fileReference);
            }
        }

        [RelayCommand]
        private async Task OpenFileDialog()
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Title = "Select UPK files", Multiselect = true, Filter = "Upk files | *.upk"
            };
            bool? success = fileDialog.ShowDialog();
            if (success != true)
            {
                return;
            }

            string[] files = fileDialog.FileNames;
            await AddFiles(files);
        }

        [RelayCommand]
        private async Task DecryptFiles()
        {
            await Task.Run(ProcessFiles);
            Process.Start("explorer.exe", OutputDirectory);
        }

        private void ProcessFiles()
        {
            var filesProcessed = 0;
            _decryptionProvider.UseKeyFile(_userConfiguration.KeysPath);
            Parallel.ForEach(FileReferences, new ParallelOptions { MaxDegreeOfParallelism = -1 }, fileReference =>
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
                var unpacked = new RLPackageUnpacker(fileStream, _decryptionProvider, FileSummarySerializer.GetDefaultSerializer());
                unpacked.Unpack(decryptedStream);
                fileReference.ProcessSuccess = unpacked.UnpackResult == UnpackResult.Success;
                Interlocked.Increment(ref filesProcessed);
            });
        }
    }

    public partial class FileReference(string filePath) : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FileName))]
        private string _filePath = filePath;

        [ObservableProperty]
        private bool _processed;

        [ObservableProperty]
        private bool _processSuccess;

        public string FileName => Path.GetFileName(FilePath);
    }
}