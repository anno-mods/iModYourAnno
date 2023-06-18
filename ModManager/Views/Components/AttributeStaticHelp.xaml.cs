using Imya.Models;
using Imya.Models.Attributes;
using Imya.Texts;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Imya.UI.Components
{
    /// <summary>
    /// Interaktionslogik für AttributeStaticHelp.xaml
    /// </summary>

    public class AttributeText 
    {
        public IAttribute Attribute { get; init; }
        public IText Text { get; init; }
    }

    public partial class AttributeStaticHelp : UserControl
    {
        public AttributeText[] Attributes { get; }

        private ITextManager TextManager;

        public AttributeStaticHelp(ITextManager textManager)
        {
            TextManager = textManager;

            Attributes = new AttributeText[]
            { };

            InitializeComponent();

            DataContext = this;
        }
    }
}
