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
using System.ComponentModel;
using System.IO.Pipes;
using System.IO;
using Newtonsoft.Json;

namespace Transmission.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string PIPENAME = "sier-g30238eq-wugb3q3-3fw3waf32";
        public ObservableCollection<Torrent> Torrents { get; } = new ObservableCollection<Torrent>();

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            if (!OpenPipe())
            {
                SendFiles();
                Close();
            }
            DoStuffAsync();
        }

        private void SendFiles()
        {
            NamedPipeClientStream pipe = new NamedPipeClientStream(PIPENAME);
            pipe.Connect();
            StreamString ss = new StreamString(pipe);
            string json = JsonConvert.SerializeObject(((App)Application.Current).PossiblePaths);
            ss.WriteString(json);
            pipe.WaitForPipeDrain();
            pipe.Close();
        }

        private bool OpenPipe()
        {
            NamedPipeServerStream pipe;
            try
            {
                pipe = new NamedPipeServerStream(PIPENAME);
            }
            catch (IOException)
            {
                return false;
            }
            RunPipeAsync(pipe);
            return true;
        }

        private async Task RunPipeAsync(NamedPipeServerStream pipe)
        {
            while (true)
            {
                await pipe.WaitForConnectionAsync();
                StreamString ss = new StreamString(pipe);
                string json = await ss.ReadStringAsync();
                var paths = JsonConvert.DeserializeObject<List<string>>(json);
                // TODO: add paths...
                System.Diagnostics.Debug.WriteLine($"other program sent files: {String.Join(", ", paths)}");
                pipe.Close();
                pipe = new NamedPipeServerStream(PIPENAME);
            }
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

    // Defines the data protocol for reading and writing strings on our stream
    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public async Task<string> ReadStringAsync()
        {
            int len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            byte[] inBuffer = new byte[len];
            await ioStream.ReadAsync(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        public async Task<int> WriteStringAsync(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            await ioStream.WriteAsync(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}
