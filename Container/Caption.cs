using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NTW.Mdi.Container
{
    public class Caption : DependencyObject
    {
        public static readonly DependencyProperty HeaderProperty =
                               DependencyProperty.Register("Header", typeof(object), typeof(Caption), new UIPropertyMetadata("Заголовок"));

        public static void SetHeader(UIElement element, object value)
        {
            element.SetValue(HeaderProperty, value);
        }
        public static object GetHeader(UIElement element)
        {
            return element.GetValue(HeaderProperty);
        }
    }
}
