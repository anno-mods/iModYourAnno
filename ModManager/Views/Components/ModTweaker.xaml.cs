using Imya.Models.ModTweaker;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Imya.Utils;

namespace Imya.UI.Components
{
    /// <summary>
    /// Interaktionslogik für ModTweaker.xaml
    /// </summary>
    public partial class ModTweaker : UserControl, INotifyPropertyChanged
    {
        public bool ShowDefaultMessage
        {
            get => _showDefaultMessage;
            set
            {
                _showDefaultMessage = value;
                OnPropertyChanged(nameof(ShowDefaultMessage));
            }
        }
        private bool _showDefaultMessage;

        public Mod CurrentMod 
        { 
            get => _currentMod;
            set 
            {
                _currentMod = value;
                OnPropertyChanged(nameof(CurrentMod));
            } 
        }
        private Mod _currentMod;

        private GameSetupManager GameSetupManager = GameSetupManager.Instance;

        public ModTweakingManager TweakingManager { get; set; } = ModTweakingManager.Instance;
        public TextManager TextManager { get; } = TextManager.Instance;

        public ModTweaker()
        {
            InitializeComponent();
            
            DataContext = this;

            IsVisibleChanged += OnVisibleChanged;
        }

        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible && CurrentMod is not null)
            {
                UpdateCurrentDisplay(CurrentMod);
            }
            else if (!IsVisible && CurrentMod is not null)
            {
                OnLeave();
            }
        }

        public void UpdateCurrentDisplay(Mod m)
        {
            CurrentMod = m;
            if (IsVisible)
            {
                TweakingManager.Save();
                TweakingManager.Clear();
                if (m is not null)
                {
                    TweakingManager.RegisterFiles(GameSetupManager.getFilesWithExtension(m, "xml"));
                }
            }
            ShowDefaultMessage = TweakingManager.HasElements();
        }

        public void OnLeave()
        {
            TweakingManager.Save();
            TweakingManager.Clear();
        }

        private void UpdateCurrentDisplay()
        { 
            
        }

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler is PropertyChangedEventHandler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
