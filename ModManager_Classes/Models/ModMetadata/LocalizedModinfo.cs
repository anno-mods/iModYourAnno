using Imya.Models.ModMetadata.ModinfoModel;
using Imya.Texts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Imya.Models.ModMetadata
{
    public class LocalizedModinfo : Modinfo
    {
        public LocalizedModinfo() 
        { }

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
