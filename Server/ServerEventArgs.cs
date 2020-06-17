using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Server //错误回调传参数
{
    public class ServerEventArgs : EventArgs
    {
        public Image Image { get; set; }
        
    }
}
