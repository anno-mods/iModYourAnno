using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Imya.Models;
using Imya.Models.ModMetadata;
using Imya.Utils;

namespace Imya.UI.Components
{
    /// <summary>
    /// Interaktionslogik für ModDescriptionDisplay.xaml
    /// </summary>
    public partial class ModDescriptionDisplay : UserControl, INotifyPropertyChanged
    {
        public Mod Mod
        {
            get => _mod;
            set
            {
                _mod = value;
                OnPropertyChanged("Mod");
            }
        }

        private Mod _mod;

        public String? Version 
        { 
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged("Version");
            }
        }
        private String? _version;
        public LocalizedText? Description { get; private set; }

        //Texts 
        private LocalizedText NoVersion = TextManager.Instance.GetText("MODDISPLAY_NO_VERSION");
        private LocalizedText NoDescription = TextManager.Instance.GetText("MODDISPLAY_NO_DESCRIPTION");

        public ModDescriptionDisplay()
        {
            DataContext = this;
            InitializeComponent();
            Version = "1.1.1.1.1.1";
        }

        //This assumes the mod has a modinfo atm.
        private void GenerateDescription(Mod mod)
        {
            Version = mod.Metadata?.Version is String ? mod.Metadata.Version : NoVersion.Text;
            Description = mod.Description is LocalizedText ? mod.Description : NoDescription;
        }

        private void Reset()
        {
            Version = null;
            Description = null;
        }

        #region INotifyPropertyChangedMembers

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler is PropertyChangedEventHandler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
