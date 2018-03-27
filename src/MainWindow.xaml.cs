﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Transmission.Api.Entities;
using Transmission.Client.ViewModel;
using Transmission.Client;
using System.ComponentModel;

namespace Transmission.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public TrulyObservableCollection<TorrentViewModel> Torrents { get; } = new TrulyObservableCollection<TorrentViewModel>();

        private TorrentViewModel _SelectedTorrent;
        public TorrentViewModel SelectedTorrent
        {
            get => _SelectedTorrent;
            set
            {
                if (_SelectedTorrent == value) return;
                _SelectedTorrent = value;
                OnPropertyChanged(nameof(SelectedTorrent));
            }
        }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            if (!FilePathPipe.Run())
                Close();
            DoStuffAsync();
        }

        private UploadViewModel _UploadVM;
        public UploadViewModel UploadVM
        {
            get => _UploadVM;
            set
            {
                if (_UploadVM == value) return;
                _UploadVM = value;
                OnPropertyChanged(nameof(UploadVM));
            }
        }

        LoginViewModel LoginVM = new LoginViewModel();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task DoStuffAsync()
        {
            var window = new Window();
            window.Content = LoginVM;
            window.ShowDialog();

            Api.Client client = new Api.Client(LoginVM.Address, LoginVM.Username, LoginVM.Password);
            UploadVM = new UploadViewModel(client);
            UploadVM.AddFiles(((App)Application.Current).PossiblePaths);
            while (true)
            {
                await UpdateShit(client);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private async Task UpdateShit(Api.Client client)
        {
            //var fields = TorrentFields.Id | TorrentFields.Name | TorrentFields.PercentDone | TorrentFields.RateDownload | TorrentFields.RateUpload | TorrentFields.Status;
            var fields = TorrentFields.All;
            var result = await client.TorrentGetAsync(fields);
            foreach (var torrent in result)
            {
                var match = Torrents.SingleOrDefault(t => t.Id == torrent.Id);
                if (match != null)
                    match.UpdateTorrent(torrent);
                else
                    Torrents.Add(new TorrentViewModel(torrent));
            }
        }
    }
}
