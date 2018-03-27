using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Transmission.Client.Converters
{
    public class BinaryPrefixConverter : PrefixConverter
    {
        private static readonly string[] _Units = new[] { "B", "kiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };
        protected override string[] Units => _Units;

        private const int _Divisor = 1024;
        protected override int Divisor => _Divisor;
    }
}
