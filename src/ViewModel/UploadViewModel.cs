using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Transmission.Client.ViewModel
{
    public class UploadViewModel : ViewModelBase
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

        public ObservableCollection<IUploadObject> Objects { get; } = new ObservableCollection<IUploadObject>();

        public string[] DropFilenames
        {
            set => AddFiles(value);
        }

        private Api.Client Client;

        public UploadViewModel(Api.Client client, IEnumerable<string> filesToAdd)
        {
            Client = client;
            AddFiles(filesToAdd);
        }

        public ICommand UploadMagnetLinkCommand => new RelayCommand(o => UploadLink(MagnetLink, DoStart), o => !String.IsNullOrWhiteSpace(MagnetLink));
        public ICommand AddMagnetLinkCommand => new RelayCommand(o => { AddMagnetLink(MagnetLink); MagnetLink = String.Empty; }, o => !String.IsNullOrWhiteSpace(MagnetLink));
        public ICommand UploadObjectsCommand => new RelayCommand(o => UploadObjectsAsync(Objects, DoStart, u => Objects.Remove(u)), o => Objects.Count > 0);

        private void UploadLink(string link, bool doStart)
        {
            Client.TorrentAddAsync(link, !doStart);
            MagnetLink = String.Empty;
        }

        private async void UploadObjectsAsync(IEnumerable<IUploadObject> objects, bool doStart, Action<IUploadObject> remove)
        {
            objects = objects.ToArray(); //copy objects so we can change source
            foreach (var @object in objects)
            {
                switch (@object)
                {
                    case UploadMagnetLink ml: await Client.TorrentAddAsync(ml.Link, !doStart); break;
                    case TorrentFile uf: await Client.TorrentAddBase64Async(uf.Base64, !doStart); break;
                    default: throw new NotImplementedException();
                }
                remove?.Invoke(@object);
            }
        }

        private void AddFiles(IEnumerable<string> files)
        {
            if (files is null) return;
            foreach (string path in files)
            {
                switch (Path.GetExtension(path))
                {
                    case ".torrent":
                        AddTorrentFile(path);
                        break;
                    case ".txt":
                        foreach (string line in File.ReadLines(path))
                            AddMagnetLink(line);
                        break;
                    default: throw new NotImplementedException();
                }

            }
        }

        private void AddMagnetLink(string magnet)
        {
            Objects.Add(new UploadMagnetLink(magnet));
        }

        private void AddTorrentFile(string path)
        {
            Objects.Add(new TorrentFile(path));
        }

        public interface IUploadObject
        {
            string Name { get; }
        }

        private class UploadMagnetLink : IUploadObject
        {
            public string Name { get; }
            public string Link { get; }

            public UploadMagnetLink(string magnetLink)
            {
                var match = System.Text.RegularExpressions.Regex.Match(magnetLink, "dn=(.*?)&");
                Name = match.Success ? match.Groups[1].Value : Link;
                Link = magnetLink;
            }
        }

        private class TorrentFile : IUploadObject
        {
            public string Name { get; }
            public string Base64 { get; }

            public TorrentFile(string path)
            {
                Name = Path.GetFileName(path);
                Base64 = Convert.ToBase64String(File.ReadAllBytes(path));

            }
        }
    }
}
