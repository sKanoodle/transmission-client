using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.Client.Model
{
    public interface ILoginDataStore
    {
        bool DoSaveData { get; set; }
        string Server { get; set; }
        string User { get; set; }
        void SaveData();
    }
}
