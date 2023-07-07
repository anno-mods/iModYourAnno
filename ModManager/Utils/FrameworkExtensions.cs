using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imya.UI.Extensions
{
    internal static class FrameworkExtensions
    {
        //see https://learn.microsoft.com/de-de/dotnet/desktop/wpf/data/how-to-find-datatemplate-generated-elements?view=netframeworkdesktop-4.8
        internal static IEnumerable<childItem> FindVisualChildren<childItem>(this UserControl userControl, DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    yield return (childItem)child;
                }
                else
                {
                    var grandchildren = userControl.FindVisualChildren<childItem>(child);
                    foreach (var grandchild in grandchildren)
                    {
                        if (grandchild != null)
                            yield return grandchild;
                    }
                }
            }
        }
    }
}
