using System;
using System.Collections.Generic;
using System.Text;

namespace ModManager_Classes.src.Metadata
{
    public class ModIdActiveTouple
    {
        public ModIdActiveTouple() { }
        public String? ModID { get; set; }
        public bool? Active { get; set; }
        public String? Version { get; set; }
    }
}
