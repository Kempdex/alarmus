using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Alarmus
{
    /// <summary>
    /// 
    /// </summary>
    public class Log
    {
        public delegate void WriteToComponent(params object[] data);
        private static List<WriteToComponent> LinkedComponents = new List<WriteToComponent>();

        private static string logFile = "Log.txt";

        public static void SetLogFileName(string name)
        {
            logFile = name;
        }

        public static void LinkComponentMethod(WriteToComponent method)
        {
            LinkedComponents.Add(method);
        } 

        /// <summary>
        /// Сообщение с заголовком DEBUG
        /// </summary>
        /// <param name="data">Массив объектов</param>
        public static void Debug(params object[] data)
        {
            LinkedComponents.ForEach(x => x(data));
            WriteMessageToFile(" [DEBUG] ", data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public static void Error(params object[] data)
        {
            LinkedComponents.ForEach(x => x(data));
            WriteMessageToFile(" [ERROR] ", data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public static void Info(params object[] data)
        {
            LinkedComponents.ForEach(x => x(data));
            WriteMessageToFile(" [INFO] ", data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public static void Warning(params object[] data)
        {
            LinkedComponents.ForEach(x => x(data));
            WriteMessageToFile(" [WARNING] ", data);
        }

        /// <summary>
        /// Записывает массив объектов в файл, предварительно приводя их к классу String
        /// </summary>
        /// <param name="header"></param>
        /// <param name="data"></param>
        private static void WriteMessageToFile(string header, params object[] data)
        {
            System.IO.File.AppendAllText(logFile, DateTime.Now.ToLongTimeString() + header);
            for (Int32 i = 0; i < data.Length; i++)
            {
                System.IO.File.AppendAllText(logFile, data[i].ToString() + " ");
            }
            System.IO.File.AppendAllText(logFile, Environment.NewLine);
        }

        
    }
}
