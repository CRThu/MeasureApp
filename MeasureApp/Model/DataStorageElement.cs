using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MeasureApp.Model
{
    public struct DataPoint
    {
        public readonly double X { get; }
        public readonly double Y { get; }

        public DataPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class DataStorageElement : NotificationObjectBase
    {
        public ObservableRangeCollection<DataPoint> DataPoints { get; set; }

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

        public IEnumerable<DataPoint> Data
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
                DataPoints = new ObservableRangeCollection<DataPoint>();
                BindingOperations.EnableCollectionSynchronization(DataPoints, locker);
            });
        }

        public event EventHandler OnDataChanged;

        public void AddXY(double x, double y)
        {
            lock (locker)
                DataPoints.Add(new DataPoint(x, y));
            OnDataChanged?.Invoke(this, new EventArgs());
        }

        public void AddY(double y)
        {
            lock (locker)
            {
                double x = DataPoints.Count == 0 ? 1 : DataPoints.Last().X + 1;
                DataPoints.Add(new DataPoint(x, y));
            }
            OnDataChanged?.Invoke(this, new EventArgs());
        }

        public void AddXY(IEnumerable<double> x, IEnumerable<double> y)
        {
            lock (locker)
                DataPoints.AddRange(x.Zip(y).Select(p => new DataPoint(p.First, p.Second)));
            OnDataChanged?.Invoke(this, new EventArgs());
        }

        public void AddY(IEnumerable<double> y)
        {
            lock (locker)
            {
                double xFirst = DataPoints.Count == 0 ? 1 : DataPoints.Last().X + 1;
                IEnumerable<double> x = Enumerable.Range(0, y.Count()).Select(n => n + xFirst);
                DataPoints.AddRange(x.Zip(y).Select(p => new DataPoint(p.First, p.Second)));
            }
            OnDataChanged?.Invoke(this, new EventArgs());
        }

        public void Clear()
        {
            lock (locker)
                DataPoints.Clear();
            OnDataChanged?.Invoke(this, new EventArgs());
        }
    }
}
