﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RL2UDK_PackageConverter
{
    public partial class Title : Form
    {
        public Title()
        {
            InitializeComponent();
        }

        private string getFilePName(string path)
        {
            return Path.GetFileName(path);
        }

        //On run button click, Start thread with the exes of RLToUdkConverter / UDKPackageCompressor and redirect c.output from those applications to outputLogConsolle
        private void runButton_Click(object sender, EventArgs e)
        {
            string currWorkingDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            Console.WriteLine(currWorkingDirectory);
            
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                    string currFile = '"' + row.Cells[0].Value.ToString() + '"';
                   // Console.WriteLine(currFile);
                
                
                if (currFile != null)
                {
                    //Process RLToUDKConverter
                    //Args 0 is inputfile
                    //Args 1 is keyFile.
                    var thread1 = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        var proc = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = currWorkingDirectory + ".\\RLToUdkConverter\\RLToUdkConverter.exe",
                                Arguments = currFile + " " + '"' + keyFileText.Text.ToString() + '"' + " " + '"' + outputFolderText.Text.ToString() + '"',
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true
                            }
                        };
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            tabViewController.SelectedTab = consoleLogTab;
                        });

                        proc.Start();
                        while (!proc.StandardOutput.EndOfStream)
                        {
                            string line = proc.StandardOutput.ReadLine();
                            // do something with line
                            this.Invoke((MethodInvoker)delegate ()
                            {
                                this.outputLogConsole.AppendText("\r\n CoreLog: " + line);
                            });

                        }

                        //compressPackageToggle
                        if (compressPackageToggle.Checked)
                        {
                            this.Invoke((MethodInvoker)delegate ()
                            {
                                this.outputLogConsole.AppendText("\r\n CoreLog: Compression Enabled, Standby for compression.");
                            });
                            //Console.WriteLine(Path.GetFileName(currFile));
                            string currFilev2 = row.Cells[0].Value.ToString();
                            var proc2 = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = currWorkingDirectory + ".\\UDKPackageCompressor\\UDKPackageCompressor.exe",
                                    Arguments = '"' + outputFolderText.Text.ToString() + "\\" + Path.GetFileNameWithoutExtension(currFilev2) + "_converted" + Path.GetExtension(currFilev2) + '"',
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    CreateNoWindow = true
                                }
                            };
                            proc2.Start();
                            while (!proc2.StandardOutput.EndOfStream)
                            {
                                string line = proc2.StandardOutput.ReadLine();
                                // do something with line
                                this.Invoke((MethodInvoker)delegate ()
                                {
                                    this.outputLogConsole.AppendText("\r\n CoreLog: " + line);
                                });

                            }
                        }

                    });
                    thread1.Start();
                    




                }
                

            }
            
        }

        //Folder Drop/Item selection below
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            //Store Items Dropped in Arr
            string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            this.dataGridView1.Rows.Clear();
            //loop and add to table
            foreach (string file in droppedFiles)
            {
                this.dataGridView1.Rows.Add(file);
                this.outputLogConsole.AppendText("\r\n Added " + getFilePName(file) + " to processing table.");
            }
        }
        private void keyFileDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void keyFileEnter(object sender, DragEventArgs e)
        {
            //Store Items Dropped in Arr
            string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

            //loop and add to table
            foreach (string file in droppedFiles)
            {
                this.keyFileText.Text = file;
                //this.dataGridView1.Rows.Add(file);
                this.outputLogConsole.AppendText("\r\n Added " + getFilePName(file) + " to keyFile.");
            }
        }

        private void outputFolderDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void outputFolderEnter(object sender, DragEventArgs e)
        {
            //Store Items Dropped in Arr
            string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

            //loop and add to table
            foreach (string file in droppedFiles)
            {
                this.outputFolderText.Text = file;
                //this.dataGridView1.Rows.Add(file);
                this.outputLogConsole.AppendText("\r\n Added " + getFilePName(file) + " to keyFile.");
            }
        }

        private void keyFileButton_Click(object sender, EventArgs e)
        {
            string oSelectedFile = "";
            System.Windows.Forms.OpenFileDialog oDlg = new System.Windows.Forms.OpenFileDialog();
            if (System.Windows.Forms.DialogResult.OK == oDlg.ShowDialog())
            {
                oSelectedFile = oDlg.FileName;
                this.keyFileText.Text = oSelectedFile;
                // Do whatever you want with oSelectedFile
            }
        }

        private void outputFolderButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog folderBrowser = new OpenFileDialog();
            // Set validate names and check file exists to false otherwise windows will
            // not let you select "Folder Selection."
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            // Always default to Folder Selection.
            folderBrowser.FileName = "Folder Selection.";
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
                this.outputFolderText.Text = folderPath;
                // ...
            }
        }
    }
}
