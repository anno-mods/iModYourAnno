using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public enum AttributeType
    {
        ModStatus = 0,
        ModCompabilityIssue = 1,
        UnresolvedDependencyIssue = 2,
        TweakedMod = 3,
        MissingModinfo = 4,
        ModContentInSubfolder = 5
    }

    public interface IAttribute
    {
        AttributeType AttributeType { get; }
        IText Description { get; }

        bool MultipleAllowed { get; }
    }
}
