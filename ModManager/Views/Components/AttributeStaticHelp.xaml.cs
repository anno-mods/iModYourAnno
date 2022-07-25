using Imya.Models;
using Imya.Models.Attributes;
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

        private TextManager TextManager = TextManager.Instance;

        public AttributeStaticHelp()
        {
            Attributes = new AttributeText[]
            {
                new() { Attribute = ModStatusAttributeFactory.Get(ModStatus.Updated), Text = TextManager.GetText("ATTRIBUTE_STATICHELP_UPDATEDMOD") },
                new() { Attribute = ModStatusAttributeFactory.Get(ModStatus.New), Text = TextManager.GetText("ATTRIBUTE_STATICHELP_NEWMOD") },
                new() { Attribute = ModStatusAttributeFactory.Get(ModStatus.Obsolete), Text = TextManager.GetText("ATTRIBUTE_STATICHELP_OBSOLETEMOD") },
                new() { Attribute = TweakedAttributeFactory.Get(), Text = TextManager.GetText("ATTRIBUTE_STATICHELP_TWEAKEDMOD") },
                new() { Attribute = MissingModinfoAttributeFactory.Get(), Text = TextManager.GetText("ATTRIBUTE_STATICHELP_NOMODINFO") },
                new() { Attribute = new ModCompabilityIssueAttribute(), Text = TextManager.GetText("ATTRIBUTE_STATICHELP_COMPABILITY")},
                new() { Attribute = new ModDependencyIssueAttribute(), Text = TextManager.GetText("ATTRIBUTE_STATICHELP_DEPENDENCY")},
            };

            InitializeComponent();

            DataContext = this;
        }
    }
}
