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

            if (value is long l)
                result = l;
            else if (value is int i)
                result = i;
            else
                throw new NotImplementedException();

            int thousands = 0;
            while (result > Divisor)
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
