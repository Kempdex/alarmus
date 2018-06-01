using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace AlarmusClient
{
    public class ClientObject
    {
        private IPEndPoint endPoint;
        private Socket socket;

        /// <summary>
        /// Базовый конструктор клиента
        /// Создает соединение с сервером по указанному адресу и порту
        /// Использует протокол TCP
        /// </summary>
        /// <param name="address">Адрес сервера</param>
        /// <param name="port">Порт сервера</param>
        public ClientObject(string address, Int32 port)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(address), port);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(endPoint);
            }
            catch(SocketException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Возвращает адрес сервера, к которому подключается клиент
        /// </summary>
        /// <returns></returns>
        public string getServerAddress()
        {
            if(endPoint == null)
            {
                return "";
            }

            return endPoint.Address.ToString();
        }

        /// <summary>
        /// Возвращется порт сервера, к которому подключается клиент
        /// </summary>
        /// <returns>Int32</returns>
        public Int32 getServerPort()
        {
            if(endPoint == null)
            {
                return 0;
            }

            return endPoint.Port;
        }

        /// <summary>
        /// Отправляет сообщение на сервер
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(Alarmus.Message message)
        {
            if (!socket.Connected)
            {
                return;
            }

            byte[] buffer = Alarmus.Serializer.Serialize(message);
            socket.Send(buffer);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReceiveResponce()
        {
            byte[] buffer = new byte[256];
            do
            {
                socket.Receive(buffer);
            }
            while (socket.Available > 0);

            Alarmus.ServerResponse response = (Alarmus.ServerResponse)Alarmus.Serializer.Deserialize(buffer);

            MessageBox.Show(response.ToString());
        }

        /// <summary>
        /// Закрывает соединение и поток клиента
        /// </summary>
        public void Disconnect()
        {
            if(!socket.Connected)
            {
                return;
            }

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
