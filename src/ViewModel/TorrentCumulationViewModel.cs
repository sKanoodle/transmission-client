using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transmission.Api.Entities;

namespace Transmission.Client.ViewModel
{
    public class TorrentCumulationViewModel : ViewModelBase
    {
        private readonly ObservableCollection<TorrentViewModel> Torrents;
        private Torrent Sums;

        public ulong TotalSize => Sums?.TotalSize ?? 0;
        public ulong HaveValid => Sums?.HaveValid ?? 0;
        public double PercentDone
        {
            get
            {
                ulong totalSize = TotalSize;
                if (totalSize == 0)
                    return 0;
                return HaveValid / (double)totalSize;
            }
        }

        public int PieceDoneCount { get; private set; }
        public int Stopped { get; private set; }
        public int Seeding { get; private set; }
        public int Downloading { get; private set; }
        public int Checking { get; private set; }
        public int Queued { get; private set; }
        public ulong HaveValidFinished { get; private set; }

        public int RateDownload => Sums?.RateDownload ?? 0;
        public int RateUpload => Sums?.RateUpload ?? 0;
        public ulong CorruptEver => Sums?.CorruptEver ?? 0;
        public ulong HaveUnchecked => Sums?.HaveUnchecked ?? 0;
        public int PeersConnected => Sums?.PeersConnected ?? 0;
        public int PeersGettingFromUs => Sums?.PeersGettingFromUs ?? 0;
        public int PeersSendingToUs => Sums?.PeersSendingToUs ?? 0;
        public int PieceCount => Sums?.PieceCount ?? 0;
        public ulong SizeWhenDone => Sums?.SizeWhenDone ?? 0;
        public ulong UploadedEver => Sums?.UploadedEver ?? 0;

        public TorrentCumulationViewModel(ObservableCollection<TorrentViewModel> torrents)
        {
            Torrents = torrents;
            Torrents.CollectionChanged += Update;
        }

        private void Update(object sender, NotifyCollectionChangedEventArgs e)
        {
            Sums = new Torrent();
            PieceDoneCount = Stopped = Seeding = Downloading = Checking = Queued = 0;
            HaveValidFinished = 0;

            foreach (TorrentViewModel torrent in Torrents)
            {
                Sums.TotalSize += torrent.TotalSize;
                Sums.HaveValid += torrent.HaveValid;
                Sums.RateDownload += torrent.RateDownload;
                Sums.RateUpload += torrent.RateUpload;
                Sums.CorruptEver += torrent.CorruptEver;
                Sums.HaveUnchecked += torrent.HaveUnchecked;
                Sums.PeersConnected += torrent.PeersConnected;
                Sums.PeersGettingFromUs += torrent.PeersGettingFromUs;
                Sums.PeersSendingToUs += torrent.PeersSendingToUs;
                Sums.PieceCount += torrent.PieceCount;
                PieceDoneCount += torrent.PiecesDone;
                Sums.SizeWhenDone += torrent.SizeWhenDone;
                Sums.UploadedEver += torrent.UploadedEver;

                switch (torrent.Status)
                {
                    case Status.Check: Checking++; break;
                    case Status.Download: Downloading++; break;
                    case Status.Seed: Seeding++; break;
                    case Status.Stopped: Stopped++; break;
                    case Status.CheckWait:
                    case Status.DownloadWait:
                    case Status.SeedWait: Queued++; break;
                    default: throw new NotImplementedException();
                }

                if (torrent.HaveValid == torrent.SizeWhenDone)
                    HaveValidFinished += torrent.HaveValid;
            }
            OnPropertyChanged(String.Empty);
        }
    }
}
