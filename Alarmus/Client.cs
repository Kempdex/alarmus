using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Alarmus;

namespace Alarmus
{
    /// <summary>
    /// 
    /// </summary>
    public class Client
    {
        public ClientType type { get; set; }
        public Socket socket { get; set; }
        public const Int32 BufferSize = 1024;
        public byte[] buffer;

        public Client(Socket socket)
        {
            this.socket = socket;
            buffer = new byte[BufferSize];
        }
    }
}
