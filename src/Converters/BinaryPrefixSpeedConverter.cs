using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.Client.Converters
{
    public class BinaryPrefixSpeedConverter : BinaryPrefixConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return base.Convert(value, targetType, parameter, culture) + "/s";
        }
    }
}
