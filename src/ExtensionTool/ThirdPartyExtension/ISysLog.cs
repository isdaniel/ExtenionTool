using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ThirdPartyExtension
{
    public interface ISysLog
    {
        void Info(string message);
        void Exception(string message,Exception ex);
    }

    public class Log4NetProvider : ISysLog
    {
        private ILog _log = LogManager.GetLogger(typeof(Log4NetProvider));

        public void Info(string message)
        {
            
            _log.Info(message);
        }

        public void Exception(string message, Exception ex)
        {
           _log.Error(message,ex);
        }
    }

    public class ConsoleProvider : ISysLog
    {
        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Exception(string message, Exception ex)
        {
            Console.WriteLine($"{message} error msg:{ex.Message}"); 
        }
    }
}
