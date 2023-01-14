using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Installation
{
    public interface IPausable
    {
        bool CanBePaused { get; set; }
        bool IsPaused { get; set; }
    }
}
