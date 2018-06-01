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
        private string userLogin;
        /// <summary>
        /// 
        /// </summary>
        private string userPassword;


        public AutorizationMessage(string login, string password)
        {
            this.type = MessageType.MSG_AUTORIZATION;
            userLogin = login;
            userPassword = password;
        }

        public string getUserLogin()
        {
            return this.userLogin;
        }

        public string getUserPassword()
        {
            return this.userPassword;
        }
    }
}
