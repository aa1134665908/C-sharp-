using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
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
            System.Windows.Forms.MessageBox.Show(message);//弹出错误信息
        }
    }
}
