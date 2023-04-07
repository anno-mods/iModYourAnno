using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public class ModCompabilityAttributeFactory
    {           
        public static IAttribute Get(IEnumerable<Mod> context)
        {
            return new GenericModContextAttribute()
            {
                AttributeType = AttributeType.ModCompabilityIssue,
                Description = new SimpleText(
                   String.Format(TextManager.Instance.GetText("ATTRIBUTE_COMPABILITYERROR").Text,
                                String.Join(',', context.Select(x => $"[{x.Category}] {x.Name}")))),
                Context = context
            };
        }
    }
}
