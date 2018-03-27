using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transmission.Api.Entities;

namespace Transmission.Client.ViewModel
{
    public class FileViewModel : ViewModelBase
    {
        public long BytesCompleted { get; }
        public int Priority { get; }

        private bool _Wanted;
        public bool Wanted
        {
            get => _Wanted;
            set => SetValue(ref _Wanted, value);
        }

        public long Length { get; }
        public string Name { get; set; }

        public FileViewModel(FileStats stats, File file)
        {
            BytesCompleted = stats.BytesCompleted;
            Priority = stats.Priority;
            Wanted = stats.Wanted;
            Length = file.Length;
            Name = file.Name;
        }

        public static IEnumerable<FileViewModel> CreateMany(IEnumerable<FileStats> stats, IEnumerable<File> files)
        {
            stats = stats.ToArray();
            files = files.ToArray();

            if (stats.Count() != files.Count())
                throw new Exception("stats and files must have the same amount of elements");

            return stats.Zip(files, (s, f) => new FileViewModel(s, f));
        }

        public override string ToString() => Name;
    }
}
