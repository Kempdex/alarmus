using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alarmus
{
    /// <summary>
    /// Тип сообщения
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Сообщение об авторизации. Содержит данные пользователя
        /// </summary>
        MSG_AUTORIZATION,
        /// <summary>
        /// Сообщение-запрос о технической поддержке. Содержит данные о неполадке
        /// </summary>
        MSG_REQUEST,
    }
}
