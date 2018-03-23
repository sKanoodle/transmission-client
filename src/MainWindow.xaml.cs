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
using System.Drawing;

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
            //var fields = TorrentFields.Id | TorrentFields.Name | TorrentFields.PercentDone | TorrentFields.RateDownload | TorrentFields.RateUpload | TorrentFields.Status | TorrentFields.Pieces;
            var fields = TorrentFields.All;
            var result = await client.TorrentGetAsync(fields);
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            foreach (var torrent in result)
            {
                watch.Restart();
                CreateTorrentPiecesBitmap(torrent);
                System.Diagnostics.Debug.WriteLine($"drew {torrent.Id} in {watch.Elapsed}");

                Torrent match = Torrents.SingleOrDefault(t => t.Id == torrent.Id);
                Torrents.Remove(match);
                Torrents.Add(torrent);
            }
        }

        private void CreateTorrentPiecesBitmap(Torrent torrent)
        {
            byte[] pieces = Convert.FromBase64String(torrent.Pieces);


            int rowCount = 100;
            Bitmap result = new Bitmap(torrent.PieceCount, rowCount, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var bitmapData = result.LockBits(new System.Drawing.Rectangle(0, 0, result.Width, result.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, result.PixelFormat);
            byte[] rowPixels = new byte[bitmapData.Stride];

            void insertPixel(int index, byte r, byte g, byte b)
            {
                rowPixels[index * 3] = b;
                rowPixels[index * 3 + 1] = g;
                rowPixels[index * 3 + 2] = r;
            }

            for (int i = 0; i < torrent.PieceCount; i++)
            {
                // read bit at specific place in byte array
                bool bit = (pieces[i / 8] & (1 << 7 - i % 8)) != 0;
                if (bit)
                    insertPixel(i, 255, 0, 0);
                else
                    insertPixel(i, 0, 255, 0);
            }
            
            for (int row = 0; row < rowCount; row++)
                unsafe
                {
                    byte* foo = ((byte*)bitmapData.Scan0.ToPointer() + row * bitmapData.Stride);
                    System.Runtime.InteropServices.Marshal.Copy(rowPixels, 0, new IntPtr(foo), rowPixels.Length);
                }

            result.UnlockBits(bitmapData);

            //Bitmap result = new Bitmap(torrent.PieceCount, rowCount, torrent.PieceCount * 3, System.Drawing.Imaging.PixelFormat.Format24bppRgb, new IntPtr(ptr));
            result.Save($@"c:\temp\bitmaps\{torrent.Id}.png");
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
