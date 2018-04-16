using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            // for each exception write its details associated with datetime 
            string[] exceptionLines = { ex.Message, DateTime.Now.ToString() };
            File.WriteAllLines(@"D:\College work\Network\project\log.txt",exceptionLines);
        }
    }
}
