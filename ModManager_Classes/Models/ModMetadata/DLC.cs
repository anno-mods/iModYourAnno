using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Imya.Models.ModMetadata
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
