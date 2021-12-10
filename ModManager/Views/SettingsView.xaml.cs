using System;
using System.Collections.Generic;
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

using Imya.Enums;
using Imya.Utils;
using Imya.Models;
using System.ComponentModel;

namespace Imya.UI.Views
{
    public class TextLanguagePair
    {
        public LocalizedText Name { get; set; }
        public ApplicationLanguage Language { get; set; }
    }

    /// <summary>
    /// Interaktionslogik für SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl, INotifyPropertyChanged
    {
        public TextLanguagePair[] Languages 
        { 
            get => _languages; 
            set
            {
                _languages = value;
                OnPropertyChanged("Languages");
            }
        }
        private TextLanguagePair[] _languages; 

        public SettingsView()
        {
            InitializeComponent();
            DataContext = this;
            Languages = new TextLanguagePair[]
            {
                new TextLanguagePair() { Name = TextManager.Instance.GetText("SETTINGS_LANG_ENGLISH"), Language = ApplicationLanguage.English},
                new TextLanguagePair() { Name = TextManager.Instance.GetText("SETTINGS_LANG_GERMAN"), Language = ApplicationLanguage.German }
            };

            LanguageSelection.SelectedItem = Languages.First(x => x.Language == TextManager.Instance.ApplicationLanguage);
        }

        public void LanguageChanged(object sender, RoutedEventArgs e)
        {
            var t = LanguageSelection.SelectedItem as TextLanguagePair;
            if (t is TextLanguagePair)
            {
                TextManager.Instance.ChangeLanguage(t.Language);
            }    
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
