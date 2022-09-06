using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models
{
    public class CompareByActiveCategoryName : IComparer<Mod>
    {
        public readonly static CompareByActiveCategoryName Default = new();

        public int Compare(Mod? x, Mod? y)
        {
            if (y is null && x is null) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            int active = y.IsActive.CompareTo(x.IsActive);
            if (active != 0)
                return active;
            int category = string.Compare(x.Modinfo?.Category.Text, y.Modinfo?.Category.Text);
            if (category != 0)
                return category;
            int name = string.Compare(x.Modinfo?.ModName.Text, y.Modinfo?.ModName.Text);
            if (name != 0)
                return name;

            return 0;
        }
    }

    public class CompareByCategoryName : IComparer<Mod>
    {
        public readonly static CompareByCategoryName Default = new();

        public int Compare(Mod? x, Mod? y)
        {
            if (y is null && x is null) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            int category = string.Compare(x.Modinfo?.Category.Text, y.Modinfo?.Category.Text);
            if (category != 0)
                return category;
            int name = string.Compare(x.Modinfo?.ModName.Text, y.Modinfo?.ModName.Text);
            if (name != 0)
                return name;

            return 0;
        }
    }

    public class CompareByFolder : IComparer<Mod>
    {
        public int Compare(Mod? x, Mod? y)
        {
            if (y is null && x is null) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            return string.Compare(x.FolderName, y.FolderName);
        }
    }
}
