using Imya.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models
{
    public interface IText
    {
        public String Text { get; }
        public void Update(ApplicationLanguage lang);

        public static IText Empty = new SimpleText("");
    }
}
