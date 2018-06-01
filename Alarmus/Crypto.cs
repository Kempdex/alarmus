using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Alarmus
{
    public class Crypto
    {
        /// <summary>
        /// Вычисляет хеш строки с помощью алгоритма MD5
        /// </summary>
        /// <param name="data">Строковое значение</param>
        /// <returns></returns>
        static public string GetMD5Hash(string data)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(data));

            byte[] result = md5.Hash;

            StringBuilder builder = new StringBuilder();
            for(Int32 i = 0; i < result.Length; i++)
            {
                builder.Append(result[i].ToString("2x"));
            }

            return builder.ToString();
        }
    }
}
