using Imya.Enums;
using Imya.Models.NotifyPropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models
{
    public class SimpleText : PropertyChangedNotifier, IText
    {
        public String Text { 
            get => _text; 
            set
            { 
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        private String _text;

        public SimpleText(String s)
        {
            Text = s;
        }

        public void Update(ApplicationLanguage lang)
        { 
        
        }

        public override string ToString()
        {
            return Text;
        }

        public static implicit operator SimpleText(string s)
        { 
            return new SimpleText(s);
        }
    }
}
