using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imya.Models.Attributes.Factories;

namespace Imya.Models.Attributes
{
    public class ModStatusAttribute : GenericAttribute
    {
        public ModStatus Status { get; init; }
    }
}
