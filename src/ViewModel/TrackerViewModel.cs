using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transmission.Api.Entities;

namespace Transmission.Client.ViewModel
{
    public class TrackerViewModel : ViewModelBase
    {
        private string _Announce;
        public string Announce
        {
            get => _Announce;
            set => SetValue(ref _Announce, value);
        }

        private TrackerState _AnnounceState;
        public TrackerState AnnounceState
        {
            get => _AnnounceState;
            set => SetValue(ref _AnnounceState, value);
        }

        private int _DownloadCount;
        public int DownloadCount
        {
            get => _DownloadCount;
            set => SetValue(ref _DownloadCount, value);
        }

        private bool _HasAnnounced;
        public bool HasAnnounced
        {
            get => _HasAnnounced;
            set => SetValue(ref _HasAnnounced, value);
        }

        private bool _HasScraped;
        public bool HasScraped
        {
            get => _HasScraped;
            set => SetValue(ref _HasScraped, value);
        }

        private string _Host;
        public string Host
        {
            get => _Host;
            set => SetValue(ref _Host, value);
        }

        private uint _ID;
        public uint ID
        {
            get => _ID;
            set => SetValue(ref _ID, value);
        }

        private int _LastAnnouncePeerCount;
        public int LastAnnouncePeerCount
        {
            get => _LastAnnouncePeerCount;
            set => SetValue(ref _LastAnnouncePeerCount, value);
        }

        private string _LastAnnounceResult;
        public string LastAnnounceResult
        {
            get => _LastAnnounceResult;
            set => SetValue(ref _LastAnnounceResult, value);
        }

        private DateTime _LastAnnounceStartTime;
        public DateTime LastAnnounceStartTime
        {
            get => _LastAnnounceStartTime;
            set => SetValue(ref _LastAnnounceStartTime, value);
        }

        private bool _LastAnnounceSucceeded;
        public bool LastAnnounceSucceeded
        {
            get => _LastAnnounceSucceeded;
            set => SetValue(ref _LastAnnounceSucceeded, value);
        }

        private DateTime _LastAnnounceTime;
        public DateTime LastAnnounceTime
        {
            get => _LastAnnounceTime;
            set => SetValue(ref _LastAnnounceTime, value);
        }

        private bool _LastAnnounceTimedOut;
        public bool LastAnnounceTimedOut
        {
            get => _LastAnnounceTimedOut;
            set => SetValue(ref _LastAnnounceTimedOut, value);
        }

        private string _LastScrapeResult;
        public string LastScrapeResult
        {
            get => _LastScrapeResult;
            set => SetValue(ref _LastScrapeResult, value);
        }

        private DateTime _LastScrapeStartTime;
        public DateTime LastScrapeStartTime
        {
            get => _LastScrapeStartTime;
            set => SetValue(ref _LastScrapeStartTime, value);
        }

        private bool _LastScrapeSucceeded;
        public bool LastScrapeSucceeded
        {
            get => _LastScrapeSucceeded;
            set => SetValue(ref _LastScrapeSucceeded, value);
        }

        private DateTime _LastScrapeTime;
        public DateTime LastScrapeTime
        {
            get => _LastScrapeTime;
            set => SetValue(ref _LastScrapeTime, value);
        }

        private bool _LastScrapeTimedOut;
        public bool LastScrapeTimedOut
        {
            get => _LastScrapeTimedOut;
            set => SetValue(ref _LastScrapeTimedOut, value);
        }

        private int _LeecherCount;
        public int LeecherCount
        {
            get => _LeecherCount;
            set => SetValue(ref _LeecherCount, value);
        }

        private DateTime _NextAnnounceTime;
        public DateTime NextAnnounceTime
        {
            get => _NextAnnounceTime;
            set => SetValue(ref _NextAnnounceTime, value);
        }

        private DateTime _NextScrapeTime;
        public DateTime NextScrapeTime
        {
            get => _NextScrapeTime;
            set => SetValue(ref _NextScrapeTime, value);
        }

        private string _ScrapeUrl;
        public string ScrapeUrl
        {
            get => _ScrapeUrl;
            set => SetValue(ref _ScrapeUrl, value);
        }

        private TrackerState _ScrapeState;
        public TrackerState ScrapeState
        {
            get => _ScrapeState;
            set => SetValue(ref _ScrapeState, value);
        }

        private int _SeederCount;
        public int SeederCount
        {
            get => _SeederCount;
            set => SetValue(ref _SeederCount, value);
        }

        private int _Tier;
        public int Tier
        {
            get => _Tier;
            set => SetValue(ref _Tier, value);
        }

        public TrackerViewModel(TrackerStats tracker) => Set(tracker);

        public void Update(TrackerStats tracker) => Set(tracker);

        public void Set(TrackerStats tracker)
        {
            Announce = tracker.Announce;
            AnnounceState = tracker.AnnounceState;
            DownloadCount = tracker.DownloadCount;
            HasAnnounced = tracker.HasAnnounced;
            HasScraped = tracker.HasScraped;
            Host = tracker.Host;
            ID = tracker.Id;
            //tracker.IsBackup;
            LastAnnouncePeerCount = tracker.LastAnnouncePeerCount;
            LastAnnounceResult = tracker.LastAnnounceResult;
            LastAnnounceStartTime = UnixToRegularTime(tracker.LastAnnounceStartTime);
            LastAnnounceSucceeded = tracker.LastAnnounceSucceeded;
            LastAnnounceTime = UnixToRegularTime(tracker.LastAnnounceTime);
            LastAnnounceTimedOut = tracker.LastAnnounceTimedOut;
            LastScrapeResult = tracker.LastScrapeResult;
            LastScrapeStartTime = UnixToRegularTime(tracker.LastScrapeStartTime);
            LastScrapeSucceeded = tracker.LastScrapeSucceeded;
            LastScrapeTime = UnixToRegularTime(tracker.LastScrapeTime);
            LastScrapeTimedOut = tracker.LastScrapeTimedOut;
            LeecherCount = tracker.LeecherCount;
            NextAnnounceTime = UnixToRegularTime(tracker.NextAnnounceTime);
            NextScrapeTime = UnixToRegularTime(tracker.NextScrapeTime);
            ScrapeUrl = tracker.Scrape;
            ScrapeState = tracker.ScrapeState;
            SeederCount = tracker.SeederCount;
            Tier = tracker.Tier;
        }
    }
}
