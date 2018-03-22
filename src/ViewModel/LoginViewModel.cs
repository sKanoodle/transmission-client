using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Transmission.Client.ViewModel
{
    class LoginViewModel
    {
        public string Address { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public ICommand Close => new RelayCommand(o => ((Window)o).Close());
    }
}
