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

        private DirectoryViewModel(string name, DirectoryViewModel[] directories, FileViewModel[] files)
        {
            Name = name;
            Files = files;
            Directories = directories;
        }

        public static DirectoryViewModel Create(string name, IEnumerable<FileViewModel> fileVMs)
        {
            // prevent IEnumerables from becoming too complicated
            fileVMs = fileVMs.ToArray();
            // no need to remove folder on root dir (name is empty here)
            if (!String.IsNullOrEmpty(name))
                foreach (var file in fileVMs)
                    file.Name = file.Name.Substring(name.Length + 1);

            // relies on the directory delimenter being '/'
            var filesWithRootDir = fileVMs.Select(f => new { File = f, Root = f.Name.Split('/')[0] });

            // if a path contains no more folder the split part and the original name are the same
            var files = filesWithRootDir
                .Where(o => o.Root == o.File.Name)
                .Select(o => o.File)
                .ToArray();

            var directories = filesWithRootDir
                .Where(o => o.Root != o.File.Name)
                .GroupBy(o => o.Root, o => o.File)
                .Select(g => Create(g.Key, g))
                .ToArray();

            //if root directory (name is string.empty) has only one subfolder, make that subfolder root
            if (!String.IsNullOrEmpty(name) || files.Any() || directories.Count() != 1)
                return new DirectoryViewModel(name, directories, files);
            return directories.Single();
        }

        public override string ToString() => Name;
    }
}
