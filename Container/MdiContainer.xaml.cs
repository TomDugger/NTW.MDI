using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NTW.Mdi.ViewModels;

namespace NTW.Mdi.Container
{
    /// <summary>
    /// Interaction logic for MdiContainer.xaml
    /// </summary>
    public partial class MdiContainer : UserControl
    {
        public MdiContainer()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //фиксируем начальную точку перемещения
            ((sender as Border).DataContext as MdiViewModel).StarPoint = Mouse.GetPosition(sender as Border);
            ((sender as Border).DataContext as MdiViewModel).MoveElement = this;
        }
    }
}
