using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.Client.Converters
{
    public class MetricPrefixConverter : PrefixConverter
    {
        private static readonly string[] _Units = new[] { "B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        protected override string[] Units => _Units;

        private const int _Divisor = 1000;
        protected override int Divisor => _Divisor;
    }
}
