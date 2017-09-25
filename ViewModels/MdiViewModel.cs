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
            CellColor = new SolidColorBrush(Colors.DodgerBlue);
        }

        #region Public
        public Brush CellColor { get; set; }

        public Point StarPoint
        {
            get { return _StarPoint; }
            set { _StarPoint = value; Debug.WriteLine(value); }
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
                        #region MyRegion
                        double localX = 0, localY = 0;
                        Tempare.Clear();
                        for (int i = 0; i < MainGrid.RowDefinitions.Count; i++)
                        {
                            for (int j = 0; j < MainGrid.ColumnDefinitions.Count; j++)
                            {
                                Rectangle rec = new Rectangle();
                                rec.Name = "_" + i + "_" + j;
                                rec.Stroke = CellColor;

                                Grid.SetColumn(rec, j);
                                Grid.SetRow(rec, i);

                                Tempare.Add(new GenTemp
                                {
                                    X = localX,
                                    Y = localY,
                                    Width = MainGrid.ColumnDefinitions[j].ActualWidth,
                                    Height = MainGrid.RowDefinitions[i].ActualHeight,
                                    Column = j,
                                    Row = i
                                });

                                MainGrid.Children.Insert(0, rec);

                                localX += MainGrid.ColumnDefinitions[j].ActualWidth;
                            }
                            localY += MainGrid.RowDefinitions[i].ActualHeight;
                        } 
                        #endregion

                        MainCanvas.Cursor = Cursors.None;
                        _MoveElement = value;
                        _MoveElement.Opacity = 0.6;
                        if (_MoveElement.ActualWidth != 0 && _MoveElement.ActualHeight != 0)
                        {
                            _MoveElement.Width = _MoveElement.ActualWidth;
                            _MoveElement.Height = _MoveElement.ActualHeight;
                        }
                        MainGrid.Children.Remove(value);

                        MainCanvas.Children.Add(value);
                        MainCanvas.Background = new SolidColorBrush(Colors.Transparent);
                        active = true;
                    }
                }
                else
                {
                    if (active)
                    {
                        MainCanvas.Cursor = null;
                        MainCanvas.Children.Remove(_MoveElement);
                        _MoveElement.Width = _MoveElement.Height = double.NaN;
                        _MoveElement.Opacity = 1;
                        //1. предворительно проверяем есть ли какой либо элемент в данной ячейке
                        var ress = from r in MainGrid.Children.OfType<MdiContainer>()
                                   where Grid.GetRow(r) == NewRowGrid && Grid.GetColumn(r) == NewColumnGrid
                                   select r;

                        var res = ResultInRowColumn(NewRowGrid, NewColumnGrid);
                        if (res.Count() == 0)
                        {
                            MainGrid.Children.Insert(MainGrid.Children.Count - 1, _MoveElement);

                            Grid.SetRow(_MoveElement, NewRowGrid);
                            Grid.SetColumn(_MoveElement, NewColumnGrid);
                        }
                        else
                        {
                            UIElement[] uis = _MoveElement.Children.ToArray();
                            foreach (UIElement ui in uis)
                                (res.ToList()[0] as MdiContainer).Children.Add(ui);
                            _MoveElement.Children.Clear();

                        }
                        _MoveElement = null;
                        MainCanvas.Background = null;

                        active = false;
                        #region Удаление разметки отображения
                        var recs = from r in MainGrid.Children.OfType<Rectangle>()
                                   select r;

                        foreach (Rectangle r in recs.ToList())
                            MainGrid.Children.Remove(r); 
                        #endregion
                    }
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
            }
        }

        public void ChangeRectangle()
        {
            var recs = from r in MainGrid.Children.OfType<Rectangle>()
                       select r;
            foreach (Rectangle rec in recs)
                if (rec.Name == "_" + NewRowGrid + "_" + NewColumnGrid)
                    rec.Fill = CellColor;
                else
                    rec.Fill = new SolidColorBrush(Colors.Transparent);
        }

        public IEnumerable<MdiContainer> ResultInRowColumn(int Row, int Column)
        {
            return from ui in MainGrid.Children.OfType<MdiContainer>()
                   where Grid.GetColumn(ui) == Column && Grid.GetRow(ui) == Row
                   select ui;
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
