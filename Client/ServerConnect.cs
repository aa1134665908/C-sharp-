using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Drawing;

namespace Client
{
   

    class ServerConnect //连接服务器
    {
        public static TcpClient server;//定义服务器端
        public static NetworkStream netStream;//把网络连接序化成流
        public static BinaryReader binaryReader;//读流
        public static BinaryWriter binaryWriter;//写流
        public static Boolean isOnline = false;//在线状态
        public static Thread listeningTask;//建立Task线程
        public static Thread transmissionTask;//建立Task线程

        public static AutoResetEvent event1 = new AutoResetEvent(false);//声明WaitOne
        public static AutoResetEvent event2 = new AutoResetEvent(false);//声明WaitOne

        private static BinaryFormatter binaryFormatter;//二进制序列化数据
        public const string CommandImage = "RECIEVEIMAGE";//与服务器端共同的连接命令
        public static void Connect(string ipAdress, int port)
        {
            SetupFields(new ServerErrorHandler("Error connecting to server"), ipAdress, port);//实现异常日志

        }

        private static void SetupFields(IErrorLogger log, string ipAdress, int port)//连接服务器
        {
            if (server != null) //如果服务器存在
            server.Close();//关闭线程
            server = new TcpClient();//新建连接
            try
            {
                event1 = new AutoResetEvent(false);//初始化WaitOne
                event2 = new AutoResetEvent(false);//初始化WaitOne
                server.Connect(ipAdress, port);//开始连接
                netStream = server.GetStream();//接收数据
                netStream.WriteTimeout = 30 * 1000;//设置连接超时时间
                binaryReader = new BinaryReader(netStream, Encoding.UTF8);//读流对象
                binaryWriter = new BinaryWriter(netStream, Encoding.UTF8);//写流对象
                binaryFormatter = new BinaryFormatter();//新建一个二进制序列化数据实例
                isOnline = true;//在线状态
            }
            catch (Exception e)
            {
                log.HandleException(e);//输出异常信息，调试用
            }


        }
        public static void Listen()
        {


            listeningTask = new Thread(RecieveTransmission);//Thread用来代替Task
            listeningTask.Start();//等待线程完成
            transmissionTask = new Thread(SendTransmission);//由于Task在.net3.5没有特性，所以要用其他的
            transmissionTask.Start();//等待线程完成
            event1.WaitOne();//等待set信号，否则不执行后面代码
            event2.WaitOne();//等待set信号，否则不执行后面代码
           Disconnect(new ServerErrorHandler("Connection ending error."));//错误回调

        }
        public static void Disconnect(IErrorLogger log)//异常回调
        {
            isOnline = false;
            try
            {
                binaryReader.Close();
                binaryWriter.Close();
                netStream.Close();
                server.Close();
                Console.WriteLine("Disconnect");

            }
            catch (Exception ex)
            {
                log.HandleException(ex);
            }

        }

        public static void RecieveTransmission()
        {
            event1.Set();//给WaitOne置信号 发出资源可用信号
        }
        public static void SendTransmission()
        {
            while (isOnline)//主动连接
            {
                try
                {
                    binaryWriter.Write(CommandImage);//发送连接命令
                    binaryWriter.Flush();//清除缓冲区
                    Bitmap screenshot = DesktopScreen.CaptureScreen(true);//开始捕捉屏幕，序列化开始

                    DesktopScreen.SerializeScreen(netStream, screenshot);//序列化截图

                    netStream.Flush();//刷新流的数据
                }
                catch (Exception e)
                {
                    isOnline = false;//异常回调
                }
                Thread.Sleep(10);
            }
            event2.Set();//给WaitOne置信号
        }
        
        
    }


}
