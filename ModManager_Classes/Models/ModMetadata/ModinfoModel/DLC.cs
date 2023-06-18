using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Imya.Enums;

namespace Imya.Models.ModMetadata.ModinfoModel
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
