using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ModManager_Classes.src.Enums;

namespace ModManager_Classes.src.Metadata
{
    public class Dlc
    {
        public Dlc() { }

        [JsonConverter(typeof(StringEnumConverter))]
        public DlcId? DLC { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DlcRequirement? Dependant { get; set; }
    }
}
