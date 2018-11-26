using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.Client.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetValue<T>(T getter, Action<T> setter, T value, [CallerMemberName]string caller = null)
        {
            if (EqualityComparer<T>.Default.Equals(getter, value))
                return false;
            setter(value);
            OnPropertyChanged(caller);
            return true;
        }

        protected bool SetValue<T>(ref T backingField, T value, [CallerMemberName]string caller = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;
            backingField = value;
            OnPropertyChanged(caller);
            return true;
        }

        protected DateTime UnixToRegularTime(int unix) => new DateTime(1970, 1, 1).AddSeconds(unix);
    }
}
