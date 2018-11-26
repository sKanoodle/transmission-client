using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.Client.Model
{
    public class WindowsLoginDataStore : ILoginDataStore
    {
        public const string SEPARATOR = "\n";

        public bool DoSaveData { get; set; }
        public string Server { get; set; }
        public string User { get; set; }

        private IsolatedStorageFile StorageFile => IsolatedStorageFile.GetUserStoreForAssembly();

        public WindowsLoginDataStore()
        {
            try
            {
                string[] args;
                using (var stream = new IsolatedStorageFileStream("login", FileMode.Open, StorageFile))
                using (var reader = new StreamReader(stream))
                    args = reader.ReadToEnd().Split(new[] { SEPARATOR }, StringSplitOptions.None);
                DoSaveData = bool.Parse(args[0]);
                Server = args[1];
                User = args[2];
            }
            catch (FileNotFoundException)
            {
                SaveData();
            }
        }

        public void SaveData()
        {
            using (var stream = new IsolatedStorageFileStream("login", FileMode.Create, StorageFile))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write($"{DoSaveData}{SEPARATOR}{(DoSaveData ? Server : String.Empty)}{SEPARATOR}{(DoSaveData ? User : String.Empty)}");
            }
        }
    }
}
