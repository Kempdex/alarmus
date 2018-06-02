using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using Alarmus;

namespace AlarmusServer
{
    /// <summary>
    /// 
    /// </summary>
    public class AsyncServer
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private static Socket listener;
        private static Int32 port;
        private static Int32 maxClientSize;
        private static List<Client> Clients = new List<Client>();

        public static void InitializeServer(Int32 _port, Int32 _maxClientSize)
        {
            port = _port;
            maxClientSize = _maxClientSize;

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            /*
             * Лист адресов содержит два адреса хоста - ipv6 и ipv4
             * Выбран второй адрес - ipv4 под индексом 1
             */
            IPAddress address = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(address, port);

            listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Log.Info("Инициализация сервера...");
                listener.Bind(localEndPoint);
                Log.Info("Сервер инициализирован. Порт: ", port.ToString(), "IP сервера: ", address.ToString());
            }
            catch(Exception e)
            {
               // MessageBox.Show("Ошибка получения локального адреса хоста сервера. Подробнее - " + e.ToString(), "Ошибка инициализации сервера");
                Log.Error("Ошибка получения локального адреса хоста сервера. Подробнее - " + e.ToString(), "Ошибка инициализации сервера");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="maxClientSize"></param>
        public static void Start()
        {
            try
            {
                Log.Info("Запуск сервера...");
                listener.Listen(maxClientSize);
                Log.Info("Сервер запущен. Максимальное количество подключений: ", maxClientSize.ToString());

                while (true)
                {
                    //
                    allDone.Reset();

                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    //Thread listenerThread = new Thread(() => listener.BeginAccept(new AsyncCallback(AcceptCallback), listener));
                    //listenerThread.Start();

                    allDone.WaitOne();
                }
            }
            catch (ThreadAbortException)
            {
                //Do nothing
            }
            catch (Exception e)
            {
                Log.Error("Ошибка при запуске сервера. Подробная информация: ", e.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                allDone.Set();

                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                Client client = new Client(handler);

                Log.Info("Пользователь подключился. Адрес пользователя: ", client.socket.LocalEndPoint.ToString());
                Clients.Add(client);

                handler.BeginReceive(client.buffer, 0, Client.BufferSize, 0, new AsyncCallback(ReadCallback), client);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
                Log.Error("Ошибка при подключении пользователя. Подробная информация: ", e.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        public static void ReadCallback(IAsyncResult ar)
        {
            try
            {
                Message msg;

                Client client = (Client)ar.AsyncState;

                Int32 bytesRead = client.socket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    msg = (Message)Serializer.Deserialize(client.buffer);

                    ParseMessage(msg, client);
                }
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.ToString());
                Log.Error("Ошибка приема данных. Подробная информация: ", e.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="handler"></param>
        public static void ParseMessage(Message msg, Client client)
        {
            switch(msg.getMessageType())
            {
                case MessageType.MSG_AUTORIZATION:
                    Autorize(msg, client);
                    break;
                case MessageType.MSG_REQUEST:
                    ProcessAnRequest(client, msg);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="client"></param>
        public static void Autorize(Message msg, Client client)
        {
            AutorizationMessage message = (AutorizationMessage)msg;

            if(DatabaseMaster.HasPassword(message))
            {
                Send(client, ServerResponse.SR_AUTORIZATION_SUCCESS);
                Log.Info("Авторизация пользователя. Логин - ", message.userLogin);
            }
            else
            {
                Send(client, ServerResponse.SR_AUTORIZATION_FAILED);
                Log.Info("Неудачная авторизация пользователя. Запрашиваемый логин - ", message.userPassword);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        public static void ProcessAnRequest(Client client, Message msg)
        {
            RequestMessage request = (RequestMessage)msg;
            Send(client, ServerResponse.SR_REQUEST_SUCCESS);
            Log.Info("Запрос технической поддержки от пользователя.");
            Log.Info("Описание проблемы: ", request.troubleData, "Тип проблемы: ", request.typeOfTrouble);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="response"></param>
        public static void Send(Client client, ServerResponse response)
        {
            try
            {
                byte[] data = Serializer.Serialize(response);

                client.socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), client);
            }
            catch(Exception e)
            {
                Log.Error("Ошибка отправки ответа клиенту. Подробная информация: ", e.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        public static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Client client = (Client)ar.AsyncState;

                Int32 bytesSend = client.socket.EndSend(ar);

                client.socket.BeginReceive(client.buffer, 0, Client.BufferSize, 0, new AsyncCallback(ReadCallback), client);
                //handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception e)
            {
                Log.Error("Ошибка отправки ответа клиенту. Подробная информация: ", e.ToString());
            }
        }

        /// <summary>
        /// Отключаем всех клиентов, завершая работу сервера
        /// </summary>
        public static void Terminate()
        {
            Clients.ForEach(client => {
                if (client.socket != null)
                {
                    client.socket.Shutdown(SocketShutdown.Both);
                    client.socket.Close();
                }
            });
        }
    }
}
