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
        private string troubleData;
        //TODO: Переделать тип проблемы в новый класс, который будет хранится на сервере и получаться во время подключения клиента к серверу
        private string typeOfTrouble;

        public RequestMessage(string troubleData, string typeOfTrouble)
        {
            this.type = MessageType.MSG_REQUEST;
            this.troubleData = troubleData;
            this.typeOfTrouble = typeOfTrouble;
        }

        public string getTroubleData()
        {
            return this.troubleData;
        }

        public string getTroubleType()
        {
            return this.typeOfTrouble;
        }
    }
}
