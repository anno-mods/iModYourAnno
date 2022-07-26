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
using Imya.UI.Popup;
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

        public GameSetupManager GameSetup { get; } = GameSetupManager.Instance;

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

        public bool HasUnsavedChanges
        {
            get => _hasUnsavedChanges;
            set {
                _hasUnsavedChanges = value;
                OnPropertyChanged(nameof(HasUnsavedChanges));
            }
        }
        private bool _hasUnsavedChanges;

        public ModTweaker()
        {
            InitializeComponent();
            DataContext = this;
            IsVisibleChanged += OnVisibleChanged;

            Application.Current.Exit += OnAppExit;

            GameSetup.GameStarted += OnGameStarted;
        }

        private void OnGameStarted()
        {
            App.Current.Dispatcher.Invoke(
            () =>
            {
                var dialog = new GenericOkayPopup()
                {
                    MESSAGE = new SimpleText("You have unsaved changes. Save now?"),
                    OK_TEXT = new SimpleText("Save Now"),
                    CANCEL_TEXT = new SimpleText("Discard Changes")
                }
                .ShowDialog();

                if (dialog is true) Save();
            });
            
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
            if (IsVisible)
            {
                LoadTweaks(mod);
            }

        }

        public void OnLeave()
        {
            if (!GameSetupManager.Instance.IsGameRunning)
            {
                Save();
            }
            else
            {
                new GenericOkayPopup() { MESSAGE = new SimpleText("Changes cannot be saved ") };
            }
        }

        public void OnAppExit(object sender, ExitEventArgs e)
        {
            OnLeave();
        }

        private void Save()
        {
            var tweaks = Tweaks;
            ThreadPool.QueueUserWorkItem(o =>
            {
                tweaks.Save();
            });
        }

        private void LoadTweaks(Mod mod)
        {
            if (HasUnsavedChanges)
            {
                HasUnsavedChanges = false;
                var dialog = new GenericOkayPopup() { 
                    MESSAGE = new SimpleText("You have unsaved changes. Save now?"),
                    OK_TEXT = new SimpleText("Save Now"),
                    CANCEL_TEXT = new SimpleText("Discard Changes")}
                .ShowDialog();
                if (dialog is true) Save();
            }

            // make sure everything is secure from access from other threads
            var currentTweaks = Tweaks;
            Tweaks = new();

            ThreadPool.QueueUserWorkItem(o =>
            {
                ModTweaks tweaks = new();

                if (mod is not null)
                    tweaks.Load(mod);

                Dispatcher.BeginInvoke(() =>
                {
                    Tweaks = tweaks;
                    HasUnsavedChanges = false;
                });
            });
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

        private void SaveButtonClicked(object sender, RoutedEventArgs e)
        {
            HasUnsavedChanges = false;
            Save();
        }

        private void ResetButtonClicked(object sender, RoutedEventArgs e)
        {
            HasUnsavedChanges = false;
            if (CurrentMod is Mod)
                LoadTweaks(CurrentMod);
        }

        private void ComboBox_Initialized(object sender, EventArgs e)
        {
            if (sender is not ComboBox box) return;
            if (box.DataContext is not IExposedModValue value) return;
            if (!value.IsEnumType) return;

            //todo fallback if the itemssource does not offer the value, we need to display what is currently in there
            box.SelectedItem = box.ItemsSource.Cast<String>().Where(x => x.Equals(value.Value)).FirstOrDefault() ?? value.Value;
            HasUnsavedChanges = false;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasUnsavedChanges = true;
            if (sender is not ComboBox box) return;
            if (box.DataContext is not IExposedModValue value) return; 
            if (!value.IsEnumType) return;

            if (box.SelectedItem is String stringval)
                value.Value = stringval;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HasUnsavedChanges = true;
            if (sender is not Slider slider) return;
            if (slider.DataContext is not IExposedModValue value) return;
            if (!value.IsSliderType) return;

            value.Value = slider.Value.ToString();
        }

        private void Slider_Initialized(object sender, EventArgs e)
        {
            if (sender is not Slider slider) return;
            if (slider.DataContext is not IExposedModValue value) return;
            if (!value.IsSliderType) return;

            if (double.TryParse(value.Value, out var slider_val))
                slider.Value = slider_val;
            HasUnsavedChanges = false;
        }

        private void CheckBox_Initialized(object sender, EventArgs e)
        {
            if (sender is not CheckBox checkBox) return;
            if (checkBox.DataContext is not IExposedModValue value) return;
            if (!value.IsToggleType) return;
            if (value is not ExposedToggleModValue togglevalue) return;

            togglevalue.IsTrue = togglevalue.Value.Equals(togglevalue.TrueValue);
            HasUnsavedChanges = false;
        }

        private void OnValueChanged(object sender, EventArgs e)
        { 
            HasUnsavedChanges = true;
        }
    }
}
