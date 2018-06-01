using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alarmus
{
    /// <summary>
    /// Базовый класс, описывающий сообщение в системе Alarmus.
    /// Его наследуют другие классы сообщений, которые расширяют доступные поля класса
    /// </summary>
    [Serializable]
    public class Message
    {
        protected MessageType type;

        /// <summary>
        /// Возвращает тип данного сообщения
        /// </summary>
        /// <returns> Тип сообщение </returns>
        /// <see cref=">MessageType"/>
        public MessageType getMessageType()
        {
            return this.type;
        }
    }
}
