﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anno.EasyMod.Mods;

namespace Imya.Models
{
    public class CompareByActiveCategoryName : IComparer<IMod>
    {
        public readonly static CompareByActiveCategoryName Default = new();

        public int Compare(IMod? x, IMod? y)
        {
            if (y is null && x is null) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            var byActive = CompareByActive.Default.Compare(x, y);
            if (byActive != 0)
                return byActive;

            return CompareByCategoryName.Default.Compare(x, y);
        }
    }

    public class CompareByActive : IComparer<IMod>
    {
        public readonly static CompareByActive Default = new();

        public int Compare(IMod? x, IMod? y)
        {
            if (y is null && x is null) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            return y.IsActive.CompareTo(x.IsActive);
        }
    }

    public class CompareByCategoryName : IComparer<IMod>
    {
        public readonly static CompareByCategoryName Default = new();

        public int Compare(IMod? x, IMod? y)
        {
            if (y is null && x is null) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            int category = string.Compare(x.Modinfo?.Category?.ToString(), y.Modinfo?.Category?.ToString());
            if (category != 0)
                return category;
            int name = string.Compare(x.Modinfo?.ModName?.ToString(), y.Modinfo?.ModName?.ToString());
            if (name != 0)
                return name;

            return 0;
        }
    }

    public class CompareByFolder : IComparer<IMod>
    {
        public readonly static CompareByFolder Default = new();

        public int Compare(IMod? x, IMod? y)
        {
            if (y is null && x is null) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            return string.Compare(x.FolderName, y.FolderName);
        }
    }

    public class ComparebyLoadOrder : IComparer<IMod>
    {
        public readonly static ComparebyLoadOrder Default = new();

        public int Compare(IMod? x, IMod? y)
        {
            //ignore inactive
            var byActive = CompareByActive.Default.Compare(x, y);
            if (byActive != 0)
                return byActive;

            var catX = GetCategory(x!);
            var catY = GetCategory(y!);

            //same category
            if (catX == catY)
            {
                if (catX != Category.NoLoadAfter)
                    CompareByLoadAfterID.Default.Compare(x, y);
                return CompareByCategoryName.Default.Compare(x, y);
            }

            //different categories

            //x is wildcard dependant: x comes last. Double wildcard is excluded by same category
            if (IsWildcardDependant(x))
                return 1;
            else if (IsWildcardDependant(y))
                return -1;
            return CompareByLoadAfterID.Default.Compare(x, y);
        }

        private enum Category { LoadAfterNoWildcard, NoLoadAfter, Wildcard }

        private Category GetCategory(IMod x)
        {
            if (x.Modinfo.LoadAfterIds is null)
                return Category.NoLoadAfter;
            if (IsWildcardDependant(x))
                return Category.Wildcard;
            else return Category.LoadAfterNoWildcard;
        }

        private bool IsWildcardDependant(IMod x)
        {
            return x.Modinfo?.LoadAfterIds?.Contains("*") ?? false;
        }

    }

    public class CompareByLoadAfterID : IComparer<IMod>
    {
        public readonly static CompareByLoadAfterID Default = new();

        public int Compare(IMod? x, IMod? y)
        {
            if (y is null && x is null) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            var xDy = x?.Modinfo?.LoadAfterIds?.Contains(y?.ModID) ?? false;
            var yDx = y?.Modinfo?.LoadAfterIds?.Contains(x?.ModID) ?? false;
            if (xDy && yDx)
                return 0;
            if (xDy)
                return 1;
            if (yDx)
                return -1;
            return 0;
        }
    }
}
