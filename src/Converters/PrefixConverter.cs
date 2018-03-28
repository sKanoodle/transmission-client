using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Transmission.Client.Converters
{
    public abstract class PrefixConverter : IValueConverter
    {
        abstract protected string[] Units { get; }
        abstract protected int Divisor { get; }

        virtual public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result;
            switch (value)
            {
                case long l: result = l; break;
                case ulong ul: result = ul; break;
                case int i: result = i; break;
                case uint ui: result = ui; break;
                default: throw new NotImplementedException();
            }
            
            int thousands = 0;
            while (result >= Divisor)
            {
                result /= Divisor;
                thousands += 1;
            }
            return $"{result:N2} {Units[thousands]}";
        }

        virtual public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
