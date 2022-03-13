using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Imya.Models;
using Imya.Utils;

namespace Imya.UI.Views
{
    /// <summary>
    /// Main view to install mods.
    /// </summary>
    public partial class InstallationView : UserControl
    {
        public InstallationView()
        {
            InitializeComponent();
        }

        private async void OnInstallFromZip(object sender, RoutedEventArgs e)
        {
            if (ModCollection.Global is null) return;

            var dialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Zip Archives (*.zip)|*.zip",
                RestoreDirectory = true, // TODO keep location separate from game path dialog, it's annoying!
                Multiselect = true
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            var modCollections = new List<ModCollection>();
            foreach (var filePath in dialog.FileNames)
            {
                Console.WriteLine($"Extract zip: {filePath}");
                var result = await ModInstaller.ExtractZipAsync(filePath, Path.Combine(Directory.GetCurrentDirectory(), Properties.Settings.Default.DownloadDir));
                if (result != null)
                    modCollections.Add(result);
            }

            // TODO
            // a dialog to select which individual mods from the zip files should be taken can be done here
            // but I feel it's rather nicer to show icons like "new" in the mod list after all is done
            // would be less interuptive and the user can remove unwanted stuff the same way

            foreach (var collection in modCollections)
            {
                Console.WriteLine($"Install zip: {collection.ModsPath}");
                await ModCollection.Global.AddAsync(collection);
                Directory.Delete(collection.ModsPath, true);
            }
        }
    }
}
