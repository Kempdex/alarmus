using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Alarmus
{
    public class StateObject
    {
        public Socket workSocket = null;

        public const Int32 BufferSize = 1024;

        public byte[] buffer = new byte[BufferSize];
    }
}
