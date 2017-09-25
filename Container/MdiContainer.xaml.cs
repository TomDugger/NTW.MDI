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
    /// <summary>
    /// Interaction logic for MdiContainer.xaml
    /// </summary>
    [ContentProperty("Children")]
    public partial class MdiContainer : UserControl
    {
        internal MdiContainer()
        {
            InitializeComponent();

            Children = new ObservableCollection<UIElement>();
            Children.CollectionChanged+=new System.Collections.Specialized.NotifyCollectionChangedEventHandler((sender, e) => {
                if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count >= 1)
                {
                    UIElement ui = e.NewItems[0] as UIElement;
                    if (Children.Count == 1)//single реализация
                    {
                        ContentList.Visibility = System.Windows.Visibility.Hidden;
                        ContentPost.Content = ui;
                    }
                    else //multy реализация
                    {
                        if (ContentPost.Visibility != System.Windows.Visibility.Hidden)
                        {
                            ContentPost.Visibility = System.Windows.Visibility.Hidden;
                            ContentList.Visibility = System.Windows.Visibility.Visible;

                            UIElement u = ContentPost.Content as UIElement;
                            ContentPost.Content = null;

                            CreateTabItem(u);
                        }

                        CreateTabItem(ui);

                        //Указания имени заголовка для привязки
                        BindingOperations.SetBinding(HeaderCaption, Label.ContentProperty, new Binding("SelectedContent.(cap:Caption.Header)") { Source = ContentList, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Count >= 1)
                {
                    if (Children.Count == 1)//multy реализация
                    {
                        ContentList.Items.Clear();
                        ContentPost.Visibility = System.Windows.Visibility.Visible;
                        ContentList.Visibility = System.Windows.Visibility.Hidden;
                        ContentPost.Content = Children[0];
                    }
                    else //single реализация
                    {
                        ContentList.Items.RemoveAt(ContentList.Items.IndexOf(GetElement(e.OldItems[0] as UIElement)));
                    }
                }
                InvalidateVisual();
            });
        }

        #region Public
        public ObservableCollection<UIElement> Children { get; set; }
        #endregion

        private TabItem GetElement(UIElement element)
        {
            return (from r in ContentList.Items.OfType<TabItem>()
                   where r.Content == element
                   select r).First();
        }

        private void CreateTabItem(UIElement element)
        {
            TabItem ti = new TabItem() { Content = element };
            BindingOperations.SetBinding(ti, TabItem.HeaderProperty, new Binding("(cap:Caption.Header)") { Source = element });
            ContentList.Items.Add(ti);
        }

        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MdiContainer mc = new MdiContainer();
            mc.Children.Add((sender as TabItem).Content as UIElement);
            Children.Remove((sender as TabItem).Content as UIElement);

            ((sender as Border).DataContext as MdiViewModel).StarPoint = Mouse.GetPosition(sender as TabItem);
            ((sender as Border).DataContext as MdiViewModel).MoveElement = mc;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //фиксируем начальную точку перемещения
            ((sender as Border).DataContext as MdiViewModel).StarPoint = Mouse.GetPosition(sender as Border);
            ((sender as Border).DataContext as MdiViewModel).MoveElement = this;
        }

        private void ContentList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MdiContainer mc = new MdiContainer();
                mc.Children.Add(ContentList.SelectedContent as UIElement);
                mc.Height = ContentList.ActualHeight;
                mc.Width = ContentList.ActualWidth;

                Children.Remove(ContentList.SelectedContent as UIElement);

                //Point p = Mouse.GetPosition(sender as TabControl);
                ((sender as TabControl).DataContext as MdiViewModel).StarPoint = Mouse.GetPosition(sender as TabControl);
                ((sender as TabControl).DataContext as MdiViewModel).MoveElement = mc;
            }
        }
    }
}
