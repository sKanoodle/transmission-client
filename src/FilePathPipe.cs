using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Transmission.Client
{
    static class FilePathPipe
    {
        private const string PIPENAME = "sier-g30238eq-wugb3q3-3fw3waf32";
        private static Action<string[]> ProcessPaths;

        /// <summary>
        /// Either runs the pipe (true) or sends the filenames to the running application (false).
        /// </summary>
        public static bool Run(Action<string[]> processPaths)
        {
            ProcessPaths = processPaths;
            bool result;
            if (result = !OpenPipe())
                SendFiles();
            return !result;
        }

        private static void SendFiles()
        {
            NamedPipeClientStream pipe = new NamedPipeClientStream(PIPENAME);
            pipe.Connect();
            StreamString ss = new StreamString(pipe);
            string json = JsonConvert.SerializeObject(((App)Application.Current).PossiblePaths);
            ss.WriteString(json);
            pipe.WaitForPipeDrain();
            pipe.Close();
        }

        /// <summary>
        /// Tries to open the pipe, when that was successful, this is the first application opened and we can run the server. Otherwise return false.
        /// </summary>
        private static bool OpenPipe()
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

        private static async Task RunPipeAsync(NamedPipeServerStream pipe)
        {
            while (true)
            {
                await pipe.WaitForConnectionAsync();
                StreamString ss = new StreamString(pipe);
                string json = await ss.ReadStringAsync();
                var paths = JsonConvert.DeserializeObject<string[]>(json);
                ProcessPaths(paths);
                System.Diagnostics.Debug.WriteLine($"other program sent files: {String.Join(", ", paths)}");
                pipe.Close();
                pipe = new NamedPipeServerStream(PIPENAME);
            }
        }

        // Defines the data protocol for reading and writing strings on our stream
        private class StreamString
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
}
