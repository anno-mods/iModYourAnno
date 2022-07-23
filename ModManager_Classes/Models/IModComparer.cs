using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models
{
    public interface IModComparer : IComparer<Mod>
    {

    }

    public class NameComparer : IModComparer
    {
        public int Compare(Mod? x, Mod? y)
        {
            if (x is null && y is not null) return -1;
            if (y is null && x is not null) return 1;
            if (y is null && x is null) return 0;

            int active = y!.IsActive.CompareTo(x.IsActive);
            if (active != 0) return active;
            int category = String.Compare(x?.Modinfo?.Category.Text, y?.Modinfo?.Category.Text);
            if (category != 0) return category;
            int name = String.Compare(x?.Modinfo?.ModName.Text, y?.Modinfo?.ModName.Text);
            if (name != 0) return name;

            return 0;
        }
    }

    public class FolderNameComparer : IModComparer
    {
        public int Compare(Mod? x, Mod? y)
        {
            if (x is null && y is not null) return -1;
            if (y is null && x is not null) return 1;
            if (y is null && x is null) return 0;

            return String.Compare(x?.FolderName, y?.FolderName);
        }
    }
}
