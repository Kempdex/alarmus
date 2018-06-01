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
        private List<StateObject> clients;

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
                Log.Info("Сервер запущен. Максимальное количество подключений:", maxClientSize.ToString());

                while (true)
                {
                    //
                    allDone.Reset();

                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    

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

                StateObject state = new StateObject();
                state.workSocket = handler;
                //Log.Info("Пользователь подключился. Адрес пользователя: ", state.workSocket.LocalEndPoint.ToString());

                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
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

                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                Int32 bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    msg = (Message)Serializer.Deserialize(state.buffer);

                    ParseMessage(msg, handler);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
                Log.Error("Ошибка приема данных. Подробная информация: ", e.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="handler"></param>
        public static void ParseMessage(Message msg, Socket handler)
        {
            switch(msg.getMessageType())
            {
                case MessageType.MSG_AUTORIZATION:
                    
                    AutorizationMessage autorization = (AutorizationMessage)msg;
                    if (DatabaseMaster.HasPassword(autorization))
                    {
                        Send(handler, ServerResponse.SR_AUTORIZATION_SUCCESS);
                        Log.Info("Пользователь авторизирован. Логин: ", autorization.getUserLogin());
                    }
                    else
                    {
                        Send(handler, ServerResponse.SR_AUTORIZATION_FAILED);
                        Log.Info("Неудачная попытка авторизации пользователя.");
                    }
                    
                    break;
                case MessageType.MSG_REQUEST:
                    Log.Info("Запрос технической поддержки от пользователя.");
                    Send(handler, ServerResponse.SR_REQUEST_SUCCESS);
                    RequestMessage request = (RequestMessage)msg;
                    Log.Info("Описание проблемы: ", request.getTroubleData(), "Тип проблемы: ", request.getTroubleType());
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="response"></param>
        public static void Send(Socket handler, ServerResponse response)
        {
            try
            {
                byte[] data = Serializer.Serialize(response);

                handler.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), handler);
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
                Socket handler = (Socket)ar.AsyncState;

                Int32 bytesSend = handler.EndSend(ar);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch(Exception e)
            {
                Log.Error("Ошибка отправки ответа клиенту. Подробная информация: ", e.ToString());
            }
        }
    }
}
