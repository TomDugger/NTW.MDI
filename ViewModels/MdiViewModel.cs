﻿using System;
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
                        MainCanvas.Cursor = Cursors.None;
                        _MoveElement = value;
                        _MoveElement.Width = _MoveElement.ActualWidth;
                        _MoveElement.Height = _MoveElement.ActualHeight;
                        MainGrid.Children.Remove(value);

                        MainCanvas.Children.Add(value);
                        MainCanvas.Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));
                        active = true;

                        //так как я ничего сверх крутого не придумал то просто заполним панель прямо на лету
                        //хоть это и не есть правильно
                        for (int i = 0; i < MainGrid.RowDefinitions.Count; i++)
                            for (int j = 0; j < MainGrid.ColumnDefinitions.Count; j++)
                            {
                                Rectangle rec = new Rectangle();
                                rec.Name = "_" + i + "_" + j;
                                rec.Fill = new SolidColorBrush(Colors.Maroon);

                                rec.Margin = new Thickness(5);

                                rec.MouseMove += new MouseEventHandler((s, e) =>
                                {
                                    (s as Rectangle).Fill = new SolidColorBrush(Colors.Yellow);
                                    string[] spl = (s as Rectangle).Name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                                    NewRowGrid = Convert.ToInt32(spl[0]);
                                    NewColumnGrid = Convert.ToInt32(spl[1]);
                                });

                                Grid.SetRow(rec, i);
                                Grid.SetColumn(rec, j);

                                MainGrid.Children.Insert(MainGrid.Children.Count - 1, rec);
                            }

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
                        var recs = from r in MainGrid.Children.OfType<Rectangle>()
                                   select r;

                        foreach (Rectangle r in recs.ToList())
                            MainGrid.Children.Remove(r); 
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