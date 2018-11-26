using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Transmission.Client.Model;

namespace Transmission.Client.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private ILoginDataStore LoginDataStore;

        public bool DoSave
        {
            get => LoginDataStore.DoSaveData;
            set => SetValue(LoginDataStore.DoSaveData, b => LoginDataStore.DoSaveData = b, value);
        }

        public string Address
        {
            get => LoginDataStore.Server;
            set => SetValue(LoginDataStore.Server, s => LoginDataStore.Server = s, value);
        }

        public string Username
        {
            get => LoginDataStore.User;
            set => SetValue(LoginDataStore.User, s => LoginDataStore.User = s, value);
        }

        public SecureString Password { get; set; }

        private string _ErrorString;
        public string ErrorString
        {
            get => _ErrorString;
            set => SetValue(ref _ErrorString, value);
        }

        public ICommand Close => new RelayCommand(o => ((Window)o).Close());

        public LoginViewModel(ILoginDataStore loginDataStore)
        {
            LoginDataStore = loginDataStore;
        }
    }
}
