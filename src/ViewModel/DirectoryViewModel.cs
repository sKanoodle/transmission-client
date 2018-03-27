using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.Client.ViewModel
{
    public class DirectoryViewModel : ViewModelBase
    {
        public DirectoryViewModel[] Directories { get; }
        public FileViewModel[] Files { get; }
        public string Name { get; }

        public DirectoryViewModel(string name, IEnumerable<FileViewModel> files)
        {
            Name = name;
            // prevent IEnumerables from becoming too complicated
            files = files.ToArray();
            // no need to remove folder on root dir (name is empty here)
            if (!String.IsNullOrEmpty(Name))
                foreach (var file in files)
                    file.Name = file.Name.Substring(Name.Length + 1);

            // relies on the directory delimenter being '/'
            var filesWithRootDir = files.Select(f => new { File = f, Root = f.Name.Split('/')[0] });

            // if a path contains no more folder the split part and the original name are the same
            Files = filesWithRootDir
                .Where(o => o.Root == o.File.Name)
                .Select(o => o.File)
                .ToArray();

            Directories = filesWithRootDir
                .Where(o => o.Root != o.File.Name)
                .GroupBy(o => o.Root, o => o.File)
                .Select(g => new DirectoryViewModel(g.Key, g))
                .ToArray();
        }

        public override string ToString() => Name;
    }
}
