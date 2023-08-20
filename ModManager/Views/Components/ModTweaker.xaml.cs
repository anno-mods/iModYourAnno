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
using Anno.EasyMod.Mods;
using Imya.Models.ModTweaker.DataModel.Tweaking;
using Imya.Services;
using Imya.Services.Interfaces;
using Imya.Texts;
using Imya.UI.Popup;
using Imya.UI.Utils;
using Imya.Utils;
using Newtonsoft.Json;

namespace Imya.UI.Components
{
    /// <summary>
    /// Interaktionslogik für ModTweaker.xaml
    /// </summary>
    public partial class ModTweaker : UserControl, INotifyPropertyChanged
    {
        public ITextManager TextManager { get; init;  }
        public ITweakService TweakManager { get; init; }
        public IGameSetupService GameSetup { get; init; }
        private readonly PopupCreator _popupCreator; 

        public IMod? CurrentMod
        {
            get => _currentMod;
            set
            {
                _currentMod = value;
                OnPropertyChanged(nameof(CurrentMod));
            }
        }
        private IMod? _currentMod;

        public ModTweaker(
            ITextManager textManager,
            ITweakService tweakService,
            IGameSetupService gameSetupService,
            PopupCreator popupCreator)
        {
            GameSetup = gameSetupService;
            TweakManager = tweakService;
            TextManager = textManager;
            _popupCreator = popupCreator;

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
        }

        public void UpdateCurrentDisplay(IMod mod)
        {
            CurrentMod = mod;
            if (IsVisible)
            {
                LoadTweaks(mod);
            }
        }

        public void OnAppExit(object sender, ExitEventArgs e)
        {
            TweakManager.Save();
        }

        private void LoadTweaks(IMod mod)
        {
            if (TweakManager.HasUnsavedChanges)
            {
                var dialog = _popupCreator.CreateSaveTweakPopup();
                dialog.ShowDialog();
            }
            TweakManager.Load(mod);
        }

        private void Reload()
        {
            if(CurrentMod is not null)
                TweakManager.Load(CurrentMod, false);
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
            TweakManager.Save();
        }

        private void ResetButtonClicked(object sender, RoutedEventArgs e)
        {
            if (CurrentMod is IMod) 
                Reload();
        }

        private void ComboBox_Initialized(object sender, EventArgs e)
        {
            if (sender is not ComboBox box) return;
            if (box.DataContext is not IExposedModValue value) return;
            if (!value.IsEnumType) return;

            //todo fallback if the itemssource does not offer the value, we need to display what is currently in there
            box.SelectedItem = box.ItemsSource.Cast<String>().Where(x => x.Equals(value.Value)).FirstOrDefault() ?? value.Value;
            TweakManager.HasUnsavedChanges = false;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TweakManager.HasUnsavedChanges = true;
            if (sender is not ComboBox box) return;
            if (box.DataContext is not IExposedModValue value) return; 
            if (!value.IsEnumType) return;

            if (box.SelectedItem is String stringval)
                value.Value = stringval;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TweakManager.HasUnsavedChanges = true;
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
            TweakManager.HasUnsavedChanges = false;
        }

        private void CheckBox_Initialized(object sender, EventArgs e)
        {
            if (sender is not CheckBox checkBox) return;
            if (checkBox.DataContext is not IExposedModValue value) return;
            if (!value.IsToggleType) return;
            if (value is not ExposedToggleModValue togglevalue) return;

            togglevalue.IsTrue = togglevalue.Value.Equals(togglevalue.TrueValue);
            TweakManager.HasUnsavedChanges = false;
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            TweakManager.HasUnsavedChanges = true;
        }
    }
}
