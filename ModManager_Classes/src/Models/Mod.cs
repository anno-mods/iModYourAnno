using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModManager_Classes.src.Metadata;
using Newtonsoft.Json;
using System.IO;
using System.Collections.ObjectModel;

namespace ModManager_Classes.src.Models
{
    public class Mod
    {
        #region fields_backing
        private string _directory_name;
        private string _name;
        private string _category;
        private bool _active;
        private bool _selected;
        private string _description;
        private string? _b64_Image;
        private ObservableCollection<ExposedModValue>? _exposedValues;
        #endregion

        //Mod filepath
        public string DirectoryName { get => _directory_name; set => _directory_name = value; }
        public string Name { get => _name; set => _name = value; }
        public string Category { get => _category; set => _category = value; }
        public bool Active { get => _active; set => _active = value; }
        public bool Selected { get => _selected; set => _selected = value; }
        public string Description { get => _description; set => _description = value; }
        //add a default image here!!!
        public string? B64_Image { get => _b64_Image; set => _b64_Image = value; }
        public ObservableCollection<ExposedModValue>? ExposedValues { get => _exposedValues; private set => _exposedValues = value; }

        public bool HasExposedValues { get => ExposedValues is ObservableCollection<ExposedModValue>; }
        public bool HasMetadata { get => Metadata is Modinfo; }
        
        //store the Modinfo data for whatever we need it later on.
        private Modinfo? Metadata;

        //this should only take in the last part (i.e. "[Gameplay] AI Shipyard" of the path.)
        public Mod(bool active, String ModName)
        {
            //if we need to trim the start dash, the mod should become inactive.
            Active = active;
            this.DirectoryName = ModName;

            //mod with Metadata
            if (TrySerializeMetadata(System.IO.Path.Combine(DirectoryName, "modinfo.json"), out var metadata))
            {
                Metadata = metadata;
                Category = Metadata?.Category?.getText() ?? "NoCategory";
                Name = Metadata?.ModName?.getText() ?? ModName;
                Description = Metadata?.Description?.getText() ?? String.Empty;
                B64_Image = Metadata?.Image;
            }
            //mod without Metadata
            else
            {
                Category = "NoCategory";
                Name = DirectoryName;
                Description = String.Empty;
                B64_Image = null;
            }
        }

        public bool TrySerializeMetadata(String MetadataFile, out Modinfo? metadata)
        {
            try
            {
                metadata = JsonConvert.DeserializeObject<Modinfo>(File.ReadAllText(MetadataFile));
                return true;
            }
            catch (JsonSerializationException e)
            {
                metadata = null;
                Console.WriteLine("Json Serialization failed: {0}", MetadataFile);
            }
            catch (IOException e)
            {
                metadata = null;
                Console.WriteLine("File not found: {0}", MetadataFile);
            }
            return false;
        }

    }
}
