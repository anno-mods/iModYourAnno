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

        public LocalizedModinfo GetLocalizedModinfo(Modinfo m, string folderName)
        {
            var localized = new LocalizedModinfo
            {
                Category = _textManager.CreateLocalizedText(m.Category, ""),
                ModName = _textManager.CreateLocalizedText(m.ModName, folderName),
                Description = _textManager.CreateLocalizedText(m.Description, null),
                KnownIssues = m.KnownIssues?.Select(x => _textManager.CreateLocalizedText(x, null)).OfType<IText>().ToArray(),
                Version = m.Version,
                ModID = m.ModID,
                IncompatibleIds = m.IncompatibleIds,
                DeprecateIds = m.DeprecateIds,
                ModDependencies = m.ModDependencies,
                DLCDependencies = m.DLCDependencies,
                CreatorName = m.CreatorName,
                CreatorContact = m.CreatorContact,
                Image = m.Image,
                LoadAfterIds = m.LoadAfterIds
            };

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
