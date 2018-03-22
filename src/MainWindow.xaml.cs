using System;
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

namespace Transmission.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Torrent> Torrents { get; } = new ObservableCollection<Torrent>();

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            DoStuffAsync();
        }

        LoginViewModel LoginVM = new LoginViewModel();

        private async Task DoStuffAsync()
        {
            var window = new Window();
            window.Content = LoginVM;
            window.ShowDialog();

            Api.Client client = new Api.Client(LoginVM.Address, LoginVM.Username, LoginVM.Password);
            while (true)
            {
                await UpdateShit(client);
                await Task.Delay(500);
            }
        }

        private async Task UpdateShit(Api.Client client)
        {
            var result = await client.TorrentGetAsync(TorrentFields.Id | TorrentFields.Name | TorrentFields.PercentDone | TorrentFields.RateDownload | TorrentFields.RateUpload | TorrentFields.Status);
            foreach (var torrent in result)
            {
                Torrent match = Torrents.SingleOrDefault(t => t.Id == torrent.Id);
                Torrents.Remove(match);
                Torrents.Add(torrent);
            }
        }
    }
}
