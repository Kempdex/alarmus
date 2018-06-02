using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alarmus
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RequestMessage : Message
    {
        public string troubleData { get; }
        //TODO: Переделать тип проблемы в новый класс, который будет хранится на сервере и получаться во время подключения клиента к серверу
        public string typeOfTrouble { get; }

        public RequestMessage(string troubleData, string typeOfTrouble)
        {
            type = MessageType.MSG_REQUEST;
            this.troubleData = troubleData;
            this.typeOfTrouble = typeOfTrouble;
        }
    }
}
