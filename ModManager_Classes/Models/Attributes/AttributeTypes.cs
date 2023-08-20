using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public static class AttributeTypes
    {
        public const String ModStatus = "ModStatus";
        public const String ModCompabilityIssue = "ModCompabilityIssue";
        public const String UnresolvedDependencyIssue = "UnresolvedDependencyIssue";
        public const String TweakedMod = "TweakedMod";
        public const String MissingModinfo = "MissingModinfo";
        public const String ModContentInSubfolder = "ModContentInSubfolder";
        public const String IssueModRemoved = "IssueModRemoved";
        public const String IssueModAccess = "IssueModAccess";
        public const String IssueNoDelete = "IssueNoDelete";
        public const String ModReplacedByIssue = "ModReplacedByIssue";
        public const String CyclicDependency = "CyclicDependency";
        public const String DlcNotOwned = "DlcNotOwned";
    }
}
