using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Alarmus
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AutorizationMessage : Message
    {
        /// <summary>
        /// 
        /// </summary>
        public string userLogin { get; }
        /// <summary>
        /// 
        /// </summary>
        public string userPassword { get; }
        /// <summary>
        /// Тип клиента, запрашивающего авторизацию
        /// </summary>
        public ClientType clientType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientType"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public AutorizationMessage(ClientType clientType, string login, string password)
        {
            type = MessageType.MSG_AUTORIZATION;
            this.clientType = clientType;
            userLogin = login;
            userPassword = password;
        }


    }
}
