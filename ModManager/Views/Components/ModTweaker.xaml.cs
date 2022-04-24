using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Imya.Models.ModTweaker;
using Imya.Utils;
using Newtonsoft.Json;

namespace Imya.UI.Components
{
    /// <summary>
    /// Interaktionslogik für ModTweaker.xaml
    /// </summary>
    public partial class ModTweaker : UserControl, INotifyPropertyChanged
    {
        public TextManager TextManager { get; } = TextManager.Instance;

        public Mod? CurrentMod
        {
            get => _currentMod;
            set
            {
                _currentMod = value;
                OnPropertyChanged(nameof(CurrentMod));
            }
        }
        private Mod? _currentMod;

        public ModTweaks Tweaks
        {
            get => _tweaks;
            private set
            {
                _tweaks = value;
                OnPropertyChanged(nameof(Tweaks));
            }
        }
        private ModTweaks _tweaks = new();

        public ModTweaker()
        {
            InitializeComponent();
            DataContext = this;
            IsVisibleChanged += OnVisibleChanged;

            Application.Current.Exit += OnAppExit;
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

        public void UpdateCurrentDisplay(Mod mod)
        {
            // make sure everything is secure from access from other threads
            var currentTweaks = Tweaks;
            Tweaks = new();

            ThreadPool.QueueUserWorkItem(o =>
                {
                    ModTweaks tweaks = new();
                    if (IsVisible)
                    {
                        currentTweaks.Save();
                        
                        if (mod is not null)
                            tweaks.Load(mod);

                        if (currentTweaks.TweakerFiles is not null)
                        {
                            foreach (TweakerFile file in currentTweaks.TweakerFiles)
                            {
                                file.TweakStorage?.Save(file.BasePath);
                            }
                        }
                    }

                    Dispatcher.BeginInvoke(() =>
                    {
                        Tweaks = tweaks;
                    });
                });
        }

        public void OnLeave()
        {
            var tweaks = Tweaks;
            Tweaks = new();
            ThreadPool.QueueUserWorkItem(o =>
                {
                    tweaks.Save();
                    foreach (TweakerFile file in tweaks.TweakerFiles)
                    {
                        file.TweakStorage?.Save(file.BasePath);
                    }
                });

            
        }

        public void OnAppExit(object sender, ExitEventArgs e)
        {
            OnLeave();
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
