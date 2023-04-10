using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public enum AttributeType
    {
        ModStatus,
        ModCompabilityIssue,
        UnresolvedDependencyIssue,
        TweakedMod,
        MissingModinfo,
        ModContentInSubfolder,
        IssueModRemoved,
        IssueModAccess,
        IssueNoDelete,
        ModReplacedByIssue,
        CyclicDependency
    }

    public interface IAttribute
    {
        AttributeType AttributeType { get; }
        IText Description { get; }

        bool MultipleAllowed { get; }
    }
}
