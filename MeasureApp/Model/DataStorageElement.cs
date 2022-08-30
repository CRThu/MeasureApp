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
        public List<DataPoint> DataPoints { get; set; }
        private object locker = new();

        public int Count => DataPoints.Count;

        public DataStorageElement()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                DataPoints = new List<DataPoint>();
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
            double x = DataPoints.Count == 0 ? 1 : DataPoints.Last().X + 1;
            AddXY(x, y);
        }

        public void AddXY(IEnumerable<double> x, IEnumerable<double> y)
        {
            lock (locker)
                DataPoints.AddRange(x.Zip(y).Select(p => new DataPoint(p.First, p.Second)));
            OnDataChanged?.Invoke(this, new EventArgs());
        }

        public void AddY(IEnumerable<double> y)
        {
            double xFirst = DataPoints.Count == 0 ? 1 : DataPoints.Last().X + 1;
            IEnumerable<double> x = Enumerable.Range(0, y.Count()).Select(n => n + xFirst);
            AddXY(x, y);
        }
    }
}
