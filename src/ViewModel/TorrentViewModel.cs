using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Transmission.Api.Entities;

namespace Transmission.Client.ViewModel
{
    public class TorrentViewModel : ViewModelBase
    {
        private DateTime _ActivityDate;
        public DateTime ActivityDate
        {
            get => _ActivityDate;
            set => SetValue(ref _ActivityDate, value);
        }

        private ulong _CorruptEver;
        public ulong CorruptEver
        {
            get => _CorruptEver;
            set => SetValue(ref _CorruptEver, value);
        }

        private TimeSpan _Eta;
        public TimeSpan Eta
        {
            get => _Eta;
            set => SetValue(ref _Eta, value);
        }

        private Func<DirectoryViewModel> _CreateFilesRootDirectory;
        public DirectoryViewModel FilesRootDirectory => _CreateFilesRootDirectory();

        private ulong _HaveUnchecked;
        public ulong HaveUnchecked
        {
            get => _HaveUnchecked;
            set => SetValue(ref _HaveUnchecked, value);
        }

        private ulong _HaveValid;
        public ulong HaveValid
        {
            get => _HaveValid;
            set => SetValue(ref _HaveValid, value);
        }

        private int _Id;
        public int Id
        {
            get => _Id;
            set => SetValue(ref _Id, value);
        }

        private string _Name;
        public string Name
        {
            get => _Name;
            set => SetValue(ref _Name, value);
        }

        public TrulyObservableCollection<PeerViewModel> Peers { get; } = new TrulyObservableCollection<PeerViewModel>();

        private int _PeersConnected;
        public int PeersConnected
        {
            get => _PeersConnected;
            set => SetValue(ref _PeersConnected, value);
        }

        private PeersFrom _PeersFrom;
        public PeersFrom PeersFrom
        {
            get => _PeersFrom;
            set => SetValue(ref _PeersFrom, value);
        }

        private int _PeersGettingFromUs;
        public int PeersGettingFromUs
        {
            get => _PeersGettingFromUs;
            set => SetValue(ref _PeersGettingFromUs, value);
        }

        private int _PeersSendingToUs;
        public int PeersSendingToUs
        {
            get => _PeersSendingToUs;
            set => SetValue(ref _PeersSendingToUs, value);
        }

        private double _PercentDone;
        public double PercentDone
        {
            get => _PercentDone;
            set => SetValue(ref _PercentDone, value);
        }

        private int _PieceCount;
        public int PieceCount
        {
            get => _PieceCount;
            set => SetValue(ref _PieceCount, value);
        }

        private string _Pieces;
        private string Pieces
        {
            get => _Pieces;
            set
            {
                if (SetValue(ref _Pieces, value))
                    PiecesGraphic = CreatePiecesBitmap();
            }
        }

        private int _PiecesDone;
        public int PiecesDone
        {
            get => _PiecesDone;
            set => SetValue(ref _PiecesDone, value);
        }

        private Bitmap _PiecesGraphic;
        public Bitmap PiecesGraphic
        {
            get => _PiecesGraphic;
            set
            {
                if (SetValue(ref _PiecesGraphic, value))
                    OnPropertyChanged(nameof(PiecesGraphicSource));
            }
        }

        public BitmapSource PiecesGraphicSource => PiecesGraphic == null 
            ? null 
            : System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                PiecesGraphic.GetHbitmap(),
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(PiecesGraphic.Width, PiecesGraphic.Height));

        private uint _PieceSize;
        public uint PieceSize
        {
            get => _PieceSize;
            set => SetValue(ref _PieceSize, value);
        }

        private int _RateDownload;
        public int RateDownload
        {
            get => _RateDownload;
            set => SetValue(ref _RateDownload, value);
        }

        private int _RateUpload;
        public int RateUpload
        {
            get => _RateUpload;
            set => SetValue(ref _RateUpload, value);
        }

        private ulong _SizeWhenDone;
        public ulong SizeWhenDone
        {
            get => _SizeWhenDone;
            set => SetValue(ref _SizeWhenDone, value);
        }

        private Status _Status;
        public Status Status
        {
            get => _Status;
            set => SetValue(ref _Status, value);
        }

        private ulong _TotalSize;
        public ulong TotalSize
        {
            get => _TotalSize;
            set => SetValue(ref _TotalSize, value);
        }

        public TrulyObservableCollection<TrackerViewModel> Trackers { get; } = new TrulyObservableCollection<TrackerViewModel>();

        private ulong _UploadedEver;
        public ulong UploadedEver
        {
            get => _UploadedEver;
            set => SetValue(ref _UploadedEver, value);
        }

        public TorrentViewModel(Torrent torrent)
        {
            SetTorrent(torrent);
        }

        /// <summary>
        /// Creates a bitmap representation of downloaded pieces and their location. SIDE EFFECT: Also calculates downloaded pieces, because we have no information about that otherwise.
        /// </summary>
        private Bitmap CreatePiecesBitmap()
        {
            int piecesDone = 0;

            if (PieceCount < 1)
                return null;

            byte[] pieces = Convert.FromBase64String(Pieces);


            int rowCount = 100;
            Bitmap result = new Bitmap(PieceCount, rowCount, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            // bitmap data has to be created so early, because apparently the byte amount in a row is always even, so we might have empty bytes at the end of a row
            // to make it easier, we just take the Stride later to calculate offsets
            var bitmapData = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, result.PixelFormat);
            byte[] rowPixels = new byte[bitmapData.Stride];

            void insertPixel(int index, byte r, byte g, byte b)
            {
                rowPixels[index * 3] = b;
                rowPixels[index * 3 + 1] = g;
                rowPixels[index * 3 + 2] = r;
            }

            for (int i = 0; i < PieceCount; i++)
            {
                // read bit at specific place in byte array (since each bit represents piece status, piece #0 is at first array index but is bit #7 in the byte)
                bool pieceLoaded = (pieces[i / 8] & (1 << 7 - i % 8)) != 0;
                if (pieceLoaded)
                {
                    piecesDone++;
                    insertPixel(i, 0, 255, 0); //green
                }
                else
                    insertPixel(i, 255, 0, 0); //red
            }

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                unsafe
                {
                    byte* rowStart = ((byte*)bitmapData.Scan0.ToPointer() + rowIndex * bitmapData.Stride);
                    System.Runtime.InteropServices.Marshal.Copy(rowPixels, 0, new IntPtr(rowStart), rowPixels.Length);
                }

            PiecesDone = piecesDone;
            result.UnlockBits(bitmapData);
            return result;
        }

        public void UpdateTorrent(Torrent torrent) => SetTorrent(torrent);

        private void SetTorrent(Torrent torrent)
        {
            ActivityDate = UnixToRegularTime(torrent.ActivityDate);
            //torrent.AddedDate;
            //torrent.BandwidthPriority;
            //torrent.Comment;
            CorruptEver = torrent.CorruptEver;
            //torrent.Creator;
            //torrent.DateCreated;
            //torrent.DesiredAvailable;
            //torrent.DoneDate;
            //torrent.DownloadDir;
            //torrent.DownloadedEver;
            //torrent.DownloadLimit;
            //torrent.DownloadLimited;
            //torrent.Error;
            //torrent.ErrorString;
            Eta = TimeSpan.FromSeconds(torrent.Eta);
            //torrent.EtaIdle;
            _CreateFilesRootDirectory = () => DirectoryViewModel.Create(String.Empty, FileViewModel.CreateMany(torrent.FileStats, torrent.Files));
            //torrent.HashString;
            HaveUnchecked = torrent.HaveUnchecked;
            HaveValid = torrent.HaveValid;
            //torrent.HonorsSessionLimits;
            Id = torrent.Id;
            //torrent.IsFinished;
            //torrent.IsPrivate;
            //torrent.IsStalled;
            //torrent.IsUploadLimited;
            //torrent.LeftUntilDone;
            //torrent.MagnetLink;
            //torrent.ManualAnnounceTime;
            //torrent.MaxConnectedPeers;
            //torrent.MetadataPercentComplete;
            Name = torrent.Name;
            //torrent.PeerLimit;
            HandlePeers(torrent.Peers);
            PeersConnected = torrent.PeersConnected;
            PeersFrom = torrent.PeersFrom;
            PeersGettingFromUs = torrent.PeersGettingFromUs;
            PeersSendingToUs = torrent.PeersSendingToUs;
            PercentDone = torrent.PercentDone;
            PieceCount = torrent.PieceCount;
            Pieces = torrent.Pieces;
            PieceSize = torrent.PieceSize;
            ////torrent.Priorities; no need, is already in FileStats
            //torrent.QueuePosition;
            RateDownload = torrent.RateDownload;
            RateUpload = torrent.RateUpload;
            //torrent.RecheckProgress;
            //torrent.SecondsDownloading;
            //torrent.SecondsSeeding;
            //torrent.SeedIdleLimit;
            //torrent.SeedIdleMode;
            //torrent.SeedRatioLimit;
            //torrent.SeedRatioMode;
            SizeWhenDone = torrent.SizeWhenDone;
            //torrent.StartDate;
            Status = torrent.Status;
            //torrent.TorrentFile;
            TotalSize = torrent.TotalSize;
            ////torrent.Trackers; not needed, all info is in  trackerstats as well. would only be useful if you need lightweight stats
            HandleTrackers(torrent.TrackerStats);
            UploadedEver = torrent.UploadedEver;
            //torrent.UploadLimit;
            //torrent.UploadRatio;
            ////torrent.Wanted; no need, is already in FileStats
            ////torrent.Webseeds; --> VM
            //torrent.WebseedsSendingToUs;
        }

        private void HandlePeers(IEnumerable<Peer> peers)
        {
            int peerHash(Peer peer) => peer.Address.GetHashCode() ^ peer.Port.GetHashCode() ^ peer.ClientName.GetHashCode();
            int peerHash2(PeerViewModel peer) => peer.Address.GetHashCode() ^ peer.Port.GetHashCode() ^ peer.ClientName.GetHashCode();

            Dictionary<int, PeerViewModel> hashToPeerVM = Peers.ToDictionary(p => peerHash2(p), p => p);
            List<int> oldHashes = hashToPeerVM.Keys.ToList();

            foreach (var peer in peers)
            {
                int hash = peerHash(peer);

                if (hashToPeerVM.TryGetValue(hash, out var match))
                {
                    oldHashes.Remove(hash);
                    match.Update(peer);
                }
                else
                    Peers.Add(new PeerViewModel(peer));
            }

            foreach (int hash in oldHashes)
                Peers.Remove(hashToPeerVM[hash]);
        }

        private void HandleTrackers(IEnumerable<TrackerStats> trackers)
        {
            Dictionary<uint, TrackerViewModel> ids = Trackers.ToDictionary(t => t.ID, t => t);
            foreach (var tracker in trackers)
            {
                if (ids.TryGetValue(tracker.Id, out var match))
                    match.Update(tracker);
                else
                    Trackers.Add(new TrackerViewModel(tracker));
            }
        }
    }
}
