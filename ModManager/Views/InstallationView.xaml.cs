using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Imya.Utils;

namespace Imya.UI.Views
{
    /// <summary>
    /// Interaktionslogik für DummyControl.xaml
    /// </summary>
    public partial class DummyControl : UserControl
    {
        public DummyControl()
        {
            InitializeComponent();
        }

        private async void OnInstallFromZip(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Zip Archives (*.zip)|*.zip",
                RestoreDirectory = true, // TODO keep location separate from game path dialog, it's annoying!
                Multiselect = true
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            var modDirectories = new List<ModInstaller.ModDirectoryTodo>();

            foreach (var filePath in dialog.FileNames)
            {
                Console.WriteLine($"Extract zip: {filePath}");
                var result = await ModInstaller.PrepareInstallZipAsync(filePath, Path.Combine(Directory.GetCurrentDirectory(), Properties.Settings.Default.DownloadDir));
                if (result != null)
                    modDirectories.Add(result);
            }

            // TODO this is the chance to select which mods to install
            // for now let's just go with installing all

            foreach (var modDirectory in modDirectories)
            {
                Console.WriteLine($"Install zip: {modDirectory.ExtractedBase}");
                await ModInstaller.FinalizeInstallAsync(modDirectory, GameSetupManager.Instance.GetModDirectory());
            }
        }
    }
}
