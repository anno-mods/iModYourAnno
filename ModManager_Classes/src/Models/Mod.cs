using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModManager_Classes.src.Metadata;
using Newtonsoft.Json;
using System.IO;

namespace ModManager_Classes.src.Models
{
    internal class Mod
    {
        #region fields_backing
        private string _path;
        private string _name;
        private string _category;
        private bool _active;
        private bool _selected;
        private string _description;
        private string _b64_Image;
        #endregion

        //Mod filepath
        public string Path { get => _path; set => _path = value; }
        public string Name { get => _name; set => _name = value; }
        public string Category { get => _category; set => _category = value; }
        public bool Active { get => _active; set => _active = value; }
        public bool Selected { get => _selected; set => _selected = value; }
        public string Description { get => _description; set => _description = value; }
        //add a default image here
        public string B64_Image { get => _b64_Image; set => _b64_Image = value; }
        
        //store the Modinfo data for whatever we need it later on.
        private Modinfo? Metadata;

        public bool hasMetadata()
        {
            return Metadata is Modinfo;
        }

        //this should only take in the last part (i.e. "[Gameplay] AI Shipyard" of the path.)
        public Mod(String DirectoryName)
        {
            //if we need to trim the start dash, the mod should become inactive.
            Active = TryTrimStart(DirectoryName, out String ModName);
            Path = ModName;

            if (TrySerializeMetadata(System.IO.Path.Combine(DirectoryName, "modinfo.json"), out var metadata))
            {
                Metadata = metadata;
            }
            else
            { 
                
            }
        }

        public bool TrySerializeMetadata(String MetadataFile, out Modinfo? metadata)
        {
            try
            {
                metadata = JsonConvert.DeserializeObject<Modinfo>(MetadataFile);
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

        /// <summary>
        /// Checks if a directory name starts with any '-' chars.
        /// </summary>
        /// <param name="s">input name</param>
        /// <param name="result">out trimmed name</param>
        /// <returns>true, if there is no dash at the start (mod is active), false, if there is one.</returns>
        private bool TryTrimStart(String s, out String result)
        {
            if (!s.StartsWith('-'))
            {
                result = s;
                return true;
            }
            while (s.StartsWith('-'))
            {
                s = s.Substring(1);
            }
            result = s;
            return false;
        }
    }
}
