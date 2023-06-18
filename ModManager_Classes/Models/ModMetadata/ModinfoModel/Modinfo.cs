using Imya.Utils;
using Newtonsoft.Json;

namespace Imya.Models.ModMetadata.ModinfoModel
{
    public class Modinfo
    {
        public Modinfo() { }
        public string? Version { get; set; }
        public string? ModID { get; set; }
        public string[]? IncompatibleIds { get; set; }
        public string[]? DeprecateIds { get; set; }
        public string[]? ModDependencies { get; set; }
        public Localized? Category { get; set; }
        public Localized? ModName { get; set; }
        public Localized? Description { get; set; }
        public Localized[]? KnownIssues { get; set; }
        public Dlc[]? DLCDependencies { get; set; }
        public string? CreatorName { get; set; }
        public string? CreatorContact { get; set; }
        public string? Image { get; set; }
        public string[]? LoadAfterIds { get; set; }
    }

    /// <summary>
    /// Localized version of Modinfo. Localized properties are readonly.
    /// </summary>
}
