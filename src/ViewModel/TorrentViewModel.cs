﻿using System;
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

        private Func<DirectoryViewModel> _CreateFilesRootDirectory;
        public DirectoryViewModel FilesRootDirectory => _CreateFilesRootDirectory();

        private long _HaveValid;
        public long HaveValid
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

        private int _PieceSize;
        public int PieceSize
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

        private Status _Status;
        public Status Status
        {
            get => _Status;
            set => SetValue(ref _Status, value);
        }

        private long _TotalSize;
        public long TotalSize
        {
            get => _TotalSize;
            set => SetValue(ref _TotalSize, value);
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

        private DateTime UnixToRegularTime(int unix) => new DateTime(1970, 1, 1).AddSeconds(unix);

        public void UpdateTorrent(Torrent torrent) => SetTorrent(torrent);

        private void SetTorrent(Torrent torrent)
        {
            ActivityDate = UnixToRegularTime(torrent.ActivityDate);
            //torrent.AddedDate;
            //torrent.BandwidthPriority;
            //torrent.Comment;
            //torrent.CorruptEver;
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
            //torrent.Eta;
            //torrent.EtaIdle;
            _CreateFilesRootDirectory = () => new DirectoryViewModel(String.Empty, FileViewModel.CreateMany(torrent.FileStats, torrent.Files));
            //torrent.HashString;
            //torrent.HaveUnchecked;
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
            ////torrent.Peers; --> again VM
            //torrent.PeersConnected;
            ////torrent.PeersFrom --> own VM
            //torrent.PeersGettingFromUs;
            //torrent.PeersSendingToUs;
            PercentDone = torrent.PercentDone;
            PieceCount = torrent.PieceCount;
            Pieces = torrent.Pieces;
            PieceSize = torrent.PieceSize;
            ////torrent.Priorities; --> VM
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
            //torrent.SizeWhenDone;
            //torrent.StartDate;
            Status = torrent.Status;
            //torrent.TorrentFile;
            TotalSize = torrent.TotalSize;
            ////torrent.Trackers; --> tracker VM
            ////torrent.TrackerStats; --> tracker VM
            //torrent.UploadedEver;
            //torrent.UploadLimit;
            //torrent.UploadRatio;
            ////torrent.Wanted; --> VM
            ////torrent.Webseeds; --> VM
            //torrent.WebseedsSendingToUs;
        }
    }
}
