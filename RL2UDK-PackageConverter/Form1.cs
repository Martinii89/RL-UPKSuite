using System.Diagnostics;
using RL2UDK_PackageConverter.Properties;

namespace RL2UDK_PackageConverter;

public partial class Title : Form
{
    public Title()
    {
        InitializeComponent();
        keyFileText.Text = Settings.Default.keyFile;
        outputFolderText.Text = Settings.Default.outputFolder;
        compressPackageToggle.Checked = Settings.Default.addCompression;
    }

    private string GetFilePName(string path)
    {
        return Path.GetFileName(path);
    }

    //On run button click, Start thread with the exes of RLToUdkConverter / UDKPackageCompressor and redirect c.output from those applications to outputLogConsolle
    private void RunButton_Click(object sender, EventArgs e)
    {
        var currWorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
        Console.WriteLine(currWorkingDirectory);
        var files = dataGridView1.Rows.Cast<DataGridViewRow>().Select(row => row.Cells[0].Value as string).ToList();
        var options = new PackageConversionOptions
        {
            Files = files,
            Compress = compressPackageToggle.Checked,
            OutputDirectory = outputFolderText.Text,
            KeysPath = keyFileText.Text
        };
        var cmd = options.GetCommandLineString();
        var thread1 = new Thread(() =>
        {
            try
            {
                Invoke(() => runButton.Enabled = false);
                Thread.CurrentThread.IsBackground = true;
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = $"{currWorkingDirectory}.\\RLToUdkConverter.exe",
                        Arguments = cmd,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                Invoke(() => tabViewController.SelectedTab = consoleLogTab);

                proc.Start();
                while (!proc.StandardOutput.EndOfStream)
                {
                    var line = proc.StandardOutput.ReadLine();
                    // do something with line
                    Invoke(() => outputLogConsole.AppendText($"\r\n CoreLog: {line}"));
                }

                Invoke(() => outputLogConsole.AppendText("\nFinished!"));
            }
            finally
            {
                Invoke(() => runButton.Enabled = true);
            }
        });
        thread1.Start();
    }

    //Folder Drop/Item selection below
    private void Form1_DragDrop(object sender, DragEventArgs e)
    {
        var droppedFiles = (string[]) e.Data?.GetData(DataFormats.FileDrop) ?? Array.Empty<string>();
        dataGridView1.Rows.Clear();
        //loop and add to table
        foreach (var file in droppedFiles)
        {
            dataGridView1.Rows.Add(file);
            outputLogConsole.AppendText($"\r\n Added {GetFilePName(file)} to processing table.");
        }
    }

    private void Form1_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effect = DragDropEffects.All;
        }
    }

    private void KeyFileDrop(object sender, DragEventArgs e)
    {
        //Store Items Dropped in Arr
        if (e.Data == null)
        {
            return;
        }

        var droppedFiles = (string[]) e.Data.GetData(DataFormats.FileDrop);

        //loop and add to table
        foreach (var file in droppedFiles)
        {
            keyFileText.Text = file;
            outputLogConsole.AppendText($"\r\n Added {GetFilePName(file)} to keyFile.");
        }

        StoreSettings();
    }

    private void StoreSettings()
    {
        Settings.Default.keyFile = keyFileText.Text;
        Settings.Default.outputFolder = outputFolderText.Text;
        Settings.Default.Save();
    }

    private void KeyFileEnter(object sender, DragEventArgs e)
    {
        if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop, false))
        {
            e.Effect = DragDropEffects.All;
        }
    }

    private void OutputFolderDrop(object sender, DragEventArgs e)
    {
        //Store Items Dropped in Arr
        if (e.Data == null)
        {
            return;
        }

        var droppedFiles = (string[]) e.Data.GetData(DataFormats.FileDrop);

        //loop and add to table
        foreach (var file in droppedFiles)
        {
            outputFolderText.Text = file;
            outputLogConsole.AppendText($"\r\n Added {GetFilePName(file)} to keyFile.");
        }

        StoreSettings();
    }

    private void OutputFolderEnter(object sender, DragEventArgs e)
    {
        if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop, false))
        {
            e.Effect = DragDropEffects.All;
        }
    }

    private void KeyFileButton_Click(object sender, EventArgs e)
    {
        var oDlg = new OpenFileDialog();
        if (DialogResult.OK != oDlg.ShowDialog())
        {
            return;
        }

        var oSelectedFile = oDlg.FileName;
        keyFileText.Text = oSelectedFile;
        StoreSettings();
        // Do whatever you want with oSelectedFile
    }

    private void OutputFolderButton_Click(object sender, EventArgs e)
    {
        var folderBrowser = new OpenFileDialog();
        // Set validate names and check file exists to false otherwise windows will
        // not let you select "Folder Selection."
        folderBrowser.ValidateNames = false;
        folderBrowser.CheckFileExists = false;
        folderBrowser.CheckPathExists = true;
        // Always default to Folder Selection.
        folderBrowser.FileName = "Folder Selection.";
        if (folderBrowser.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        var folderPath = Path.GetDirectoryName(folderBrowser.FileName);
        outputFolderText.Text = folderPath;
        StoreSettings();
    }

    private void OnCompressPackageChange(object sender, EventArgs e)
    {
        var checkBox = sender as CheckBox;
        Settings.Default.addCompression = checkBox.Checked;
        StoreSettings();
    }
}