using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    class ServerErrorHandler : IErrorLogger
    {
        string message;
        public ServerErrorHandler(string message)
        {
            this.message = message;
        }
        public void HandleException(Exception ex)
        {
            //System.Windows.Forms.MessageBox.Show(message);
            Console.WriteLine(message);
        }
    }
}
