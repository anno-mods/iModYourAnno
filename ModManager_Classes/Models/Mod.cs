﻿using Imya.Models.ModMetadata;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Imya.Models
{
    public class Mod : INotifyPropertyChanged
    {
        #region fields_backing
        private string _directory_name;
        private LocalizedText _name;
        private LocalizedText _category;
        private bool _active;
        private bool _selected;
        private LocalizedText _description;
        private string? _b64_Image;
        private ObservableCollection<ExposedModValue>? _exposedValues;
        #endregion

        //Mod filepath
        public string DirectoryName { get => _directory_name; set => _directory_name = value; }
        public LocalizedText Name { get => _name; set => _name = value; }
        public LocalizedText Category { get => _category; set => _category = value; }
        public bool Active 
        { 
            get => _active;
            set
            {
                _active = value;
                OnPropertyChanged("Active");
            }
        
        }
        public bool Selected 
        { 
            get => _selected; 
            set
            { 
                _selected = value;
                OnPropertyChanged("Selected");
            }
        }

        public LocalizedText Description { get => _description; set => _description = value; }
        //add a default image here!!!
        public string? B64_Image { get => _b64_Image; set => _b64_Image = value; }
        public ObservableCollection<ExposedModValue>? ExposedValues { get => _exposedValues; private set => _exposedValues = value; }

        public bool HasExposedValues { get => ExposedValues is ObservableCollection<ExposedModValue>; }
        public bool HasMetadata { get => Metadata is Modinfo; }
        
        //store the Modinfo data for whatever we need it later on.
        private Modinfo? Metadata;

        //this should only take in the last part (i.e. "[Gameplay] AI Shipyard" of the path.)
        public Mod (bool active, String ModName, Modinfo? metadata)
        {
            //if we need to trim the start dash, the mod should become inactive.
            Active = active;
            DirectoryName = ModName;

            //mod with Metadata
            if (metadata is Modinfo)
            {
                Metadata = metadata;
                Category = (Metadata.Category is Localized) ? new LocalizedText(Metadata.Category) : new LocalizedText("NoCategory");
                Name = (Metadata.ModName is Localized) ? new LocalizedText(Metadata.ModName) : new LocalizedText(ModName);
                Description = (Metadata.Description is Localized) ? new LocalizedText(Metadata.Description) : new LocalizedText("No Description provided!");
                B64_Image = Metadata?.Image;
            }
            //mod without Metadata
            else
            {
                bool matches = TryMatchToNamingPattern(DirectoryName, out var _category, out var _name);
                Category = new LocalizedText(matches ? _category : "NoCategory");
                Name = new LocalizedText(matches ? _name : DirectoryName);
                Description = new LocalizedText(String.Empty);
                B64_Image = null;
            }
        }

        private bool TryMatchToNamingPattern(String DirectoryName, out String Category, out String Name)
        {
            String CategoryPattern = @"[[][a-z]+[]]";
            Category = Regex.Match(DirectoryName, CategoryPattern, RegexOptions.IgnoreCase).Value.TrimStart('[').TrimEnd(']');

            String NamePattern = @"[^]]*";
            Name = Regex.Match(DirectoryName, NamePattern, RegexOptions.RightToLeft).Value.TrimStart(' ');

            return !Name.Equals("") && !Category.Equals("");
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