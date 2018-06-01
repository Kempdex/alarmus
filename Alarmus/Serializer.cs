using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Alarmus
{
    /// <summary>
    /// 
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// Переводит объект в последовательность битов
        /// </summary>
        /// <param name="obj">Любой тип данных</param>
        /// <returns></returns>
        static public byte[] Serialize(object obj)
        {
            if(obj == null)
            {
                return null;
            }

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        /// <summary>
        /// Переводит последовательность битов в объект
        /// </summary>
        /// <param name="arrBytes">Массив байтов</param>
        /// <returns></returns>
        static public object Deserialize(byte[] arrBytes)
        {
            if(arrBytes == null)
            {
                return null;
            }

            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            ms.Write(arrBytes, 0, arrBytes.Length);
            ms.Seek(0, SeekOrigin.Begin);
            object obj = bf.Deserialize(ms);

            return obj;
        }
    }
}
