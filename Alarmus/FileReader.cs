using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Alarmus
{
    public class FileReader
    {
        public static string GetAllLinesFromFile(string path)
        {
            return File.ReadAllText(path);
        }

        
    }
}
