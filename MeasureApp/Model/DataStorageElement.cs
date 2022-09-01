using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MeasureApp.Model
{
    //public struct DataPoint
    //{
    //    public readonly double X { get; }
    //    public readonly double Y { get; }

    //    public DataPoint(double x, double y)
    //    {
    //        X = x;
    //        Y = y;
    //    }
    //}

    public class DataStorageElement : NotificationObjectBase
    {
        public ObservableRangeCollection<View.Control.Point> DataPoints { get; set; }

        public IEnumerable<double> X
        {
            get
            {
                lock (locker)
                    return DataPoints.Select(p => p.X);
            }
        }

        public IEnumerable<double> Y
        {
            get
            {
                lock (locker)
                    return DataPoints.Select(p => p.Y);
            }
        }

        public IEnumerable<View.Control.Point> Data
        {
            get
            {
                lock (locker)
                    return DataPoints;
            }
        }

        private readonly object locker = new();

        public int Count => DataPoints.Count;

        public DataStorageElement()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                DataPoints = new ObservableRangeCollection<View.Control.Point>();
                BindingOperations.EnableCollectionSynchronization(DataPoints, locker);
            });
        }

        public event NotifyCollectionChangedEventHandler OnDataChanged;

        public void AddXY(double x, double y)
        {
            lock (locker)
                DataPoints.Add(new View.Control.Point(x, y));
        }

        public void AddY(double y)
        {
            lock (locker)
            {
                double x = DataPoints.Count == 0 ? 1 : DataPoints.Last().X + 1;
                DataPoints.Add(new View.Control.Point(x, y));
            }
        }

        public void AddXY(IEnumerable<double> x, IEnumerable<double> y)
        {
            lock (locker)
                DataPoints.AddRange(x.Zip(y).Select(p => new View.Control.Point(p.First, p.Second)));
        }

        public void AddY(IEnumerable<double> y)
        {
            lock (locker)
            {
                double xFirst = DataPoints.Count == 0 ? 1 : DataPoints.Last().X + 1;
                IEnumerable<double> x = Enumerable.Range(0, y.Count()).Select(n => n + xFirst);
                DataPoints.AddRange(x.Zip(y).Select(p => new View.Control.Point(p.First, p.Second)));
            }
        }

        public void Clear()
        {
            lock (locker)
                DataPoints.Clear();
        }
    }
}
