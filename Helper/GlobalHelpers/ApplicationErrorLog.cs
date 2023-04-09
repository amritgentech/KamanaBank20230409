using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.GlobalHelpers
{
    public static class ApplicationErrorLog
    {
        public static StreamWriter error(Exception exception)
        {
            // Get error and log objects
            string errorFilepath = "c:\\log\\logs.txt";

            if (!File.Exists(errorFilepath))
            {
                try
                {
                    File.Create(errorFilepath).Dispose();
                }
                catch 
                {

                }
              
            }
            var log = new StreamWriter(errorFilepath);
            var templog = log;
            // Write out what you need and close the file
            log.WriteLine(exception.Message);
            log.Close();
            return templog;
        }
    }
}
