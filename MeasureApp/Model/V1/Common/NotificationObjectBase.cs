using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.Common
{
    public class NotificationObjectBase : INotifyPropertyChanged, INotifyCollectionChanged
    {
        // Property Changed
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            object propertyName = GetPropertyName(action);
            RaisePropertyChanged(propertyName as string);
        }

        private object GetPropertyName<T>(Expression<Func<T>> action)
        {
            MemberExpression expression = (MemberExpression)action.Body;
            string propertyName = expression.Member.Name;
            return propertyName;
        }

        // Collection Changed
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        protected void FireCollectionResetNotification()
        {
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
