using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using NTW.Mdi.Container;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;

namespace NTW.Mdi.ViewModels
{
    public class MdiViewModel : INotifyPropertyChanged
    {
        #region Generic
        private class GenTemp
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public int Column { get; set; }
            public int Row { get; set; }
        }
        #endregion

        #region Private
        private double _X = 0;
        private double _Y = 0;
        private MdiContainer _MoveElement;
        private Grid MainGrid;
        private Canvas MainCanvas;
        private Point _StarPoint;
        private bool active = false;
        private int NewRowGrid = 0;
        private int NewColumnGrid = 0;
        private List<GenTemp> Tempare = new List<GenTemp>(1);
        #endregion

        public MdiViewModel()
        {

        }

        public MdiViewModel(Grid mgrid, Canvas mcanvas)
        {
            MainGrid = mgrid;
            MainCanvas = mcanvas;
        }

        #region Public
        public Point StarPoint
        {
            get { return _StarPoint; }
            set { _StarPoint = value; }
        }

        public MdiContainer MoveElement 
        {
            set {
                if (value != null)
                {
                    if (!active)
                    {
                        //так как я ничего сверх крутого не придумал то просто заполним панель прямо на лету
                        //хоть это и не есть правильно
                        double localX = 0, localY = 0;
                        Tempare.Clear();
                        for (int i = 0; i < MainGrid.RowDefinitions.Count; i++)
                        {
                            for (int j = 0; j < MainGrid.ColumnDefinitions.Count; j++)
                            {
                                Rectangle rec = new Rectangle();
                                rec.Name = "_" + i + "_" + j;
                                rec.Fill = new SolidColorBrush(Colors.Maroon);

                                Canvas.SetLeft(rec, localX);
                                Canvas.SetTop(rec, localY);

                                rec.Width = MainGrid.ColumnDefinitions[j].ActualWidth;
                                rec.Height = MainGrid.RowDefinitions[i].ActualHeight;

                                rec.MouseEnter += new MouseEventHandler((s, e) =>
                                {
                                    string[] spl = (s as Rectangle).Name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                                    if(NewRowGrid == Convert.ToInt32(spl[0]) && NewColumnGrid == Convert.ToInt32(spl[1]))
                                    {
                                        (s as Rectangle).Fill = new SolidColorBrush(Colors.Yellow);
                                    }
                                });

                                rec.MouseLeave += new MouseEventHandler((s, e) =>
                                {
                                    (s as Rectangle).Fill = new SolidColorBrush(Colors.Maroon);
                                });

                                localX += MainGrid.ColumnDefinitions[j].ActualWidth;

                                Tempare.Add(new GenTemp { X = localX, Y = localY, Width = MainGrid.ColumnDefinitions[j].ActualWidth, Height = MainGrid.RowDefinitions[i].ActualHeight, Column = j, Row = i });
                                
                                MainCanvas.Children.Add(rec);
                            }
                            localY += MainGrid.RowDefinitions[i].ActualHeight;
                        }

                        MainCanvas.Cursor = Cursors.None;
                        _MoveElement = value;
                        _MoveElement.Width = _MoveElement.ActualWidth;
                        _MoveElement.Height = _MoveElement.ActualHeight;
                        MainGrid.Children.Remove(value);

                        MainCanvas.Children.Add(value);
                        MainCanvas.Background = new SolidColorBrush(Colors.Transparent);
                        active = true;

                    #if DEBUG
                        Console.WriteLine("Установка выделенного объекта");
                    #endif
                    }
                }
                else
                {
                    if (active)
                    {
                        MainCanvas.Cursor = null;
                        MainCanvas.Children.Remove(_MoveElement);
                        MainGrid.Children.Insert(MainGrid.Children.Count - 1, _MoveElement);
                        _MoveElement.Width = _MoveElement.Height = double.NaN;
                        Grid.SetRow(_MoveElement, NewRowGrid);
                        Grid.SetColumn(_MoveElement, NewColumnGrid);
                        _MoveElement = null;
                        MainCanvas.Background = null;

                        active = false;
                        #region Удаление разметки отображения
                        var recs = from r in MainCanvas.Children.OfType<Rectangle>()
                                   select r;

                        foreach (Rectangle r in recs.ToList())
                            MainCanvas.Children.Remove(r); 
                        #endregion
                    }
                    #if DEBUG
                    Console.WriteLine("выделенный элемент очищен");
                    #endif
                }
            }
        }

        public double X
        {
            get { return _X - _StarPoint.X; }
            set {

                foreach(GenTemp gt in Tempare)
                    if (value >= gt.X && value <= gt.X + gt.Width)
                    {
                        NewColumnGrid = gt.Column;
                        break;
                    }

                _X = value; Change("X");
                #if DEBUG
                Console.WriteLine("x => " + value); 
                #endif
            }
        }

        public double Y
        {
            get { return _Y - _StarPoint.Y; }
            set {
                foreach(GenTemp gt in Tempare)
                    if (value >= gt.Y && value <= gt.Y + gt.Height)
                    {
                        NewRowGrid = gt.Row;
                        break;
                    }
                _Y = value; Change("Y"); 
                #if DEBUG
                Console.WriteLine("y => " + value); 
                #endif
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        public void Change(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
