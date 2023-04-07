using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Attributes
{
    public class CyclicDependencyAttributeFactory
    {
        public static IAttribute Get(IEnumerable<Mod> context)
        {
            return new GenericModContextAttribute()
            {
                AttributeType = AttributeType.CyclicDependency,
                Description = new SimpleText(
                   String.Format(TextManager.Instance.GetText("ATTRIBUTE_CYCLIC_DEPENDENCY").Text,
                                String.Join(',', context.Select(x => $"[{x.Category}] {x.Name}")))),
                Context = context
            };
        }
    }
}
