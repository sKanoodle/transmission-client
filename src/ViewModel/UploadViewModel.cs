using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Transmission.Client.ViewModel
{
    class UploadViewModel : ViewModelBase
    {
        private string _MagnetLink;
        public string MagnetLink
        {
            get => _MagnetLink;
            set
            {
                if (_MagnetLink == value) return;
                _MagnetLink = value;
                OnPropertyChanged(nameof(MagnetLink));
            }
        }

        private bool _DoStart = true;
        public bool DoStart
        {
            get => _DoStart;
            set
            {
                if (_DoStart == value) return;
                _DoStart = value;
                OnPropertyChanged(nameof(DoStart));
            }
        }

        private Api.Client Client;

        public UploadViewModel(Api.Client client)
        {
            Client = client;
        }

        public ICommand Upload => new RelayCommand(o => UploadLink(MagnetLink, DoStart), o => !String.IsNullOrWhiteSpace(MagnetLink));

        private void UploadLink(string link, bool doStart)
        {
            Client.TorrentAdd(link, !doStart);
        }
    }
}
