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

namespace Imya_UI.Components
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

        public String? Version { get; private set; }
        public bool StringVisible { get => Version is String; }

        public String? Description { get; private set; }
        private bool HasDescription { get => Description is String; }

        //Texts 
        private LocalizedText NoVersion = TextManager.Instance.GetText("MODDISPLAY_NO_VERSION");
        private LocalizedText NoDescription = TextManager.Instance.GetText("MODDISPLAY_NO_DESCRIPTION");

        public ModDescriptionDisplay()
        {
            InitializeComponent();
        }

        private void GenerateDescription(Modinfo metadata)
        {
            Version = metadata.Version is String ? metadata.Version : NoVersion.Text;
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
