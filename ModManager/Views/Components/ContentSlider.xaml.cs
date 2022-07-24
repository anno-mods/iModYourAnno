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

namespace Imya.UI.Views.Components
{
    /// <summary>
    /// Interaktionslogik für ContentSlider.xaml
    /// </summary>
    public class ContentSliderBase : Slider { }

    public partial class ContentSlider : ContentSliderBase
    {
        public static readonly DependencyProperty ContentProperty =
              DependencyProperty.Register("Content", typeof(object),
            typeof(ContentSlider), new UIPropertyMetadata(String.Empty));

        public ContentSlider() : base()
        {

        }

        public String Content
        {
            get
            {
                return (String) GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }
    }
}
