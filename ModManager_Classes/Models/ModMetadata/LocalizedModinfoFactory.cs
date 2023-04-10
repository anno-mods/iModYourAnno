using Imya.Models.ModMetadata.ModinfoModel;
using Imya.Texts;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Imya.Models.ModMetadata
{
    public class LocalizedModinfoFactory
    {
        private readonly ITextManager _textManager; 

        public LocalizedModinfoFactory(
            ITextManager textManager)
        {
            _textManager = textManager;
        }

        public LocalizedModinfo GetLocalizedModinfo(Modinfo m)
        {
            var localized = new LocalizedModinfo()
            {
                Category = m.Category is not null ? _textManager.CreateLocalizedText(m.Category) : new SimpleText(""),
                ModName = m.ModName is not null ? _textManager.CreateLocalizedText(m.ModName) : new SimpleText(""),
                Description = m?.Description is not null ? _textManager.CreateLocalizedText(m.Description) : null,
                KnownIssues = m?.KnownIssues is not null ? m.KnownIssues?.Where(x => x is not null).Select(x => _textManager.CreateLocalizedText(x)).ToArray() : null
            };

            localized.Version = m?.Version;
            localized.ModID = m?.ModID;
            localized.IncompatibleIds = m?.IncompatibleIds;
            localized.DeprecateIds = m?.DeprecateIds;
            localized.ModDependencies = m?.ModDependencies;
            localized.DLCDependencies = m?.DLCDependencies;
            localized.CreatorName = m?.CreatorName;
            localized.CreatorContact = m?.CreatorContact;
            localized.Image = m?.Image;
            localized.LoadAfterIds = m?.LoadAfterIds;

            return localized;
        }

        public LocalizedModinfo GetDummyModinfo(string foldername)
        {
            bool matches = MatchNameCategory(foldername, out var category, out var name);
            var modName = new SimpleText(matches ? name : foldername);
            var modCategory = matches ? new SimpleText(category) : _textManager.GetText("MODLIST_NOCATEGORY");

            return new LocalizedModinfo()
            {
                Category = modCategory,
                ModName = modName
            };
        }

        private bool MatchNameCategory(string folderName, out string category, out string name)
        {
            string CategoryPattern = @"[[][a-z]+[]]";
            category = Regex.Match(folderName, CategoryPattern, RegexOptions.IgnoreCase).Value.TrimStart('[').TrimEnd(']');

            string NamePattern = @"[^]]*";
            name = Regex.Match(folderName, NamePattern, RegexOptions.RightToLeft).Value.TrimStart(' ');

            return !name.Equals("") && !category.Equals("");
        }
    }
}
