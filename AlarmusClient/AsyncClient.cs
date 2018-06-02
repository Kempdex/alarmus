using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Windows;
using Alarmus;


namespace AlarmusClient
{
    public class AsyncClient
    {
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        public static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private static Socket client;
        public static bool isConnected;

        private static ServerResponse serverResponse;

        public static void Connect(string address, Int32 port)
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(address);
                IPAddress ipAddress = IPAddress.Parse(address);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                isConnected = false;

                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne(500);
            }
            catch(FormatException)
            {
                MessageBox.Show("Введен неверный адрес сервера");
                Log.Error("Введен неверный адрес сервера. Подключение невозможно");
            }
            catch(Exception e)
            {
                MessageBox.Show("Невозможно подключиться к серверу. Возможно сервер выключен или вы находитесь не в сети.", "Ошибка подключения");
                //MessageBox.Show(e.ToString());
                Log.Error("Невозможно подключиться к серверу.", e.ToString());   
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public static void SendMessage(Message msg)
        {
            try
            {
                /*if(!isConnected)
                {
                    Log.Error("Отправка сообщения не удалась из-за отсутствия подключения к серверу", "Тип сообщения: ", msg.getMessageType());
                    return;
                }*/

                Log.Debug("First. Is client connected? - ", client.Connected);

                /*
                 * Поставил таймауты на пару секунд, вместо половины секунды
                 * 
                 * */
                Send(client, msg);
                sendDone.WaitOne(2000); //Таймаут на две секунды

                Receive(client);
                receiveDone.WaitOne(2000); //Таймаут на две секунды

                Log.Debug("Second. Is client connected? - ", client.Connected);
            }
            catch(Exception e)
            {
                MessageBox.Show("Невозможно связаться с сервером.", "Ошибка подключения");
               // MessageBox.Show(e.ToString());
                Log.Error("Невозможно связаться с сервером", e.ToString());
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ServerResponse GetServerResponse()
        {
            return serverResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SetToNullServerResponse()
        {
            serverResponse = ServerResponse.SR_NULL;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void ParseServerResponse()
        {
            switch(serverResponse)
            {
                case Alarmus.ServerResponse.SR_AUTORIZATION_SUCCESS:
                    MessageBox.Show("Autorization success");
                    break;
                case Alarmus.ServerResponse.SR_AUTORIZATION_FAILED:
                    MessageBox.Show("Autorization failed");
                    break;
                case Alarmus.ServerResponse.SR_REQUEST_SUCCESS:
                    MessageBox.Show("Request success");
                    break;
                case Alarmus.ServerResponse.SR_REQUEST_FAILED:
                    MessageBox.Show("Request failed");
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);

                connectDone.Set();
                isConnected = true;
            }
            catch(Exception e)
            {
                MessageBox.Show("Невозможно подключиться к серверу. Возможно сервер выключен или вы находитесь не в сети.", "Ошибка подключения");
                Log.Error("Невозможно подключиться к серверу", e.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        private static void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject();
                state.workSocket = client;

                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch(Exception e)
            {
                MessageBox.Show("Ошибка отправки запроса.");
                Log.Error("Ошибка приема данных. Подробная информация: ", e.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive(ar);

                if(bytesRead > 0)
                {
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    serverResponse = (ServerResponse)Serializer.Deserialize(state.buffer);
                    receiveDone.Set();
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Ошибка отправки запроса.");
                Log.Error("Ошибка приема данных. Подробная информация: ", e.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        private static void Send(Socket client, Message msg)
        {
            byte[] data = Serializer.Serialize(msg);

            client.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), client);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                int bytesSent = client.EndSend(ar);

                // MessageBox.Show("Sent " + bytesSent + "bytes to server.");

                sendDone.Set();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Disconnect()
        {
            if(client == null)
            {
                Log.Debug("Client is null");
                return;
            }
            if(!client.Connected)
            {
                Log.Debug("Client isn't connected");
                return;
            }

            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}
