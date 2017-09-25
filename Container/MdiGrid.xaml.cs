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
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NTW.Mdi.Container
{
    [ContentProperty("Children")]
    public partial class MdiGrid : UserControl
    {
        #region Public
        public ObservableCollection<UIElement> Children { get; set; }

        public Brush CellColor { get { return (this.DataContext as MdiViewModel).CellColor; } set { (this.DataContext as MdiViewModel).CellColor = value; } }
        #endregion

        public MdiGrid()
        {
            InitializeComponent();

            //не знаю правильно ли это
            Grid.SetRowSpan(MainCanvas, int.MaxValue);
            Grid.SetColumnSpan(MainCanvas, int.MaxValue);

            MdiViewModel view = new MdiViewModel(MainGrid, MainCanvas);

            this.DataContext = view;

            Children = new ObservableCollection<UIElement>();
            Children.CollectionChanged += new NotifyCollectionChangedEventHandler((s, e) => {

                if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count >= 1)
                {
                    MdiContainer mc = new MdiContainer();//контейнер отображения
                    UIElement ui = e.NewItems[0] as UIElement;//сам пользовательский элемент

                    BindingOperations.SetBinding(mc, Grid.RowProperty, new Binding("(Grid.Row)") { Source = ui});
                    BindingOperations.SetBinding(mc, Grid.ColumnProperty, new Binding("(Grid.Column)") { Source = ui });
                    BindingOperations.SetBinding(mc, Grid.RowSpanProperty, new Binding("(Grid.RowSpan)") { Source = ui });
                    BindingOperations.SetBinding(mc, Grid.ColumnSpanProperty, new Binding("(Grid.ColumnSpan)") { Source = ui });

                    mc.Children.Add(ui);
                    MainGrid.Children.Insert(MainGrid.Children.Count != 0? MainGrid.Children.Count - 1 : 0, mc);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Count >= 1)
                {
                    UIElement omc = (UIElement)e.OldItems[0];
                    MainGrid.Children.Remove(omc);
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    MainGrid.Children.Clear();
                }
                InvalidateVisual();
            });
        }

        public RowDefinitionCollection Rows
        {
            get { return MainGrid.RowDefinitions; }
        }

        public ColumnDefinitionCollection Columns
        {
            get { return MainGrid.ColumnDefinitions; }
        }

        private void MainGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point p = Mouse.GetPosition(sender as IInputElement);
            (this.DataContext as MdiViewModel).X = p.X;
            (this.DataContext as MdiViewModel).Y = p.Y;

            (this.DataContext as MdiViewModel).ChangeRectangle();
        }

        private void MainGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            (this.DataContext as MdiViewModel).MoveElement = null;
        }
    }
}
