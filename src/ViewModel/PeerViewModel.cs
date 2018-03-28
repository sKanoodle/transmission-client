using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transmission.Api.Entities;

namespace Transmission.Client.ViewModel
{
    public class PeerViewModel : ViewModelBase
    {
        private string _Address;
        public string Address
        {
            get => _Address;
            set => SetValue(ref _Address, value);
        }

        private bool _ClientIsChoked;
        public bool ClientIsChoked
        {
            get => _ClientIsChoked;
            set => SetValue(ref _ClientIsChoked, value);
        }

        private bool _ClientIsInterested;
        public bool ClientIsInterested
        {
            get => _ClientIsInterested;
            set => SetValue(ref _ClientIsInterested, value);
        }

        private string _ClientName;
        public string ClientName
        {
            get => _ClientName;
            set => SetValue(ref _ClientName, value);
        }

        private string _FlagStr;
        public string FlagStr
        {
            get => _FlagStr;
            set => SetValue(ref _FlagStr, value);
        }

        private bool _IsDownloadingFrom;
        public bool IsDownloadingFrom
        {
            get => _IsDownloadingFrom;
            set => SetValue(ref _IsDownloadingFrom, value);
        }

        private bool _IsEncrypted;
        public bool IsEncrypted
        {
            get => _IsEncrypted;
            set => SetValue(ref _IsEncrypted, value);
        }

        private bool _IsIncoming;
        public bool IsIncoming
        {
            get => _IsIncoming;
            set => SetValue(ref _IsIncoming, value);
        }

        private bool _IsUploadingTo;
        public bool IsUploadingTo
        {
            get => _IsUploadingTo;
            set => SetValue(ref _IsUploadingTo, value);
        }

        private bool _IsUTP;
        public bool IsUTP
        {
            get => _IsUTP;
            set => SetValue(ref _IsUTP, value);
        }

        private bool _PeerIsChoked;
        public bool PeerIsChoked
        {
            get => _PeerIsChoked;
            set => SetValue(ref _PeerIsChoked, value);
        }

        private bool _PeerIsInterested;
        public bool PeerIsInterested
        {
            get => _PeerIsInterested;
            set => SetValue(ref _PeerIsInterested, value);
        }

        private int _Port;
        public int Port
        {
            get => _Port;
            set => SetValue(ref _Port, value);
        }

        private double _Progress;
        public double Progress
        {
            get => _Progress;
            set => SetValue(ref _Progress, value);
        }


        private int _RateToClient;
        public int RateToClient
        {
            get => _RateToClient;
            set => SetValue(ref _RateToClient, value);
        }

        private int _RateToPeer;
        public int RateToPeer
        {
            get => _RateToPeer;
            set => SetValue(ref _RateToPeer, value);
        }

        public PeerViewModel(Peer peer)
        {
            Set(peer);
        }

        public void Update(Peer peer) => Set(peer);

        private void Set(Peer peer)
        {
            Address = peer.Address;
            ClientIsChoked = peer.ClientIsChoked;
            ClientIsInterested = peer.ClientIsInterested;
            ClientName = peer.ClientName;
            FlagStr = peer.FlagStr;
            IsDownloadingFrom = peer.IsDownloadingFrom;
            IsEncrypted = peer.IsEncrypted;
            IsIncoming = peer.IsIncoming;
            IsUploadingTo = peer.IsUploadingTo;
            IsUTP = peer.IsUTP;
            PeerIsChoked = peer.PeerIsChoked;
            PeerIsInterested = peer.PeerIsInterested;
            Port = peer.Port;
            Progress = peer.Progress;
            RateToClient = peer.RateToClient;
            RateToPeer = peer.RateToPeer;
        }
    }
}
