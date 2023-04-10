using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Installation
{
    public enum InstallationStatus 
    { 
        NotStarted, 
        Unpacking, 
        MovingFiles, 
        Downloading 
    }
}
