using Imya.Utils;
using Newtonsoft.Json;

namespace Imya.Models.ModMetadata
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

        public LocalizedModinfo GetLocalized(string name) => new (name, this);
    }

    /// <summary>
    /// Localized version of Modinfo. Localized properties are readonly.
    /// </summary>
    public class LocalizedModinfo : Modinfo
    {
        public LocalizedModinfo(string name, Modinfo? modinfo)
        {
            Version = modinfo?.Version;
            ModID = modinfo?.ModID;
            IncompatibleIds = modinfo?.IncompatibleIds;
            DeprecateIds = modinfo?.DeprecateIds;
            ModDependencies = modinfo?.ModDependencies;
            DLCDependencies = modinfo?.DLCDependencies;
            CreatorName = modinfo?.CreatorName;
            CreatorContact = modinfo?.CreatorContact;
            Image = modinfo?.Image;

            // localize
            Category = (modinfo?.Category is not null) ? TextManager.CreateLocalizedText(modinfo.Category) : TextManager.Instance["MODLIST_NOCATEGORY"];
            ModName = (modinfo?.ModName is not null) ? TextManager.CreateLocalizedText(modinfo.ModName) : new SimpleText(name);
            Description = (modinfo?.Description is not null) ? TextManager.CreateLocalizedText(modinfo.Description) : null;
            KnownIssues = (modinfo?.KnownIssues is not null) ? modinfo.KnownIssues.Where(x => x is not null).Select(x => TextManager.CreateLocalizedText(x)).ToArray() : null;
        }

        /// <summary>
        /// Localized mod name or folder name as default.
        /// </summary>
        public new IText ModName { get; init; }

        /// <summary>
        /// Localized category or "NoCategory" as default.
        /// </summary>
        public new IText Category { get; init; }

        public new IText? Description { get; init; }
        public new IText[]? KnownIssues { get; init; }
    }
}
