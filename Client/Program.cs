using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        public static string ipaddress ="192.168.160.144";//服务器ip
        public static int port = 2020;//服务器端口
        public static int sleepsconds = 1000*5;//连接失败等待时间
        static void Main(string[] args)
        {
            while (true)
            { 
            ServerConnect.Connect(ipaddress, port);//尝试连接
            if (ServerConnect.isOnline)//如果在线
            { 
                ServerConnect.Listen(); //开始连接
            }
            Thread.Sleep(sleepsconds);//如果连接失败则睡眠
            }
            
        }
    }
}
