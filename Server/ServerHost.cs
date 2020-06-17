using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace Server
{
    class ServerHost
    {
        public const string CommandImage = "RECIEVEIMAGE";//接收的图片命令，要与客户端一致
        private static IPAddress serverIP;//定义ip格式
        private static int serverPort;//监听的端口
        private static TcpListener server;//定义服务器端
        private static TcpClient client;//定义客户端
        public static Boolean isOnline = false;//在线状态
        private static Boolean connectionTerminated = true;//连接是否终结
        private static BinaryFormatter binaryFormatter;//二进制序列化数据
        private static NetworkStream netStream;//把网络连接序化成流
        private static BinaryWriter binaryWriter;//写流
        private static BinaryReader binaryReader;//读流
        private static Task listeningTask;//建立Task线程
        private static Task transmissionTask;//建立Task线程
        public static Form1 parentForm;//父窗口
        public static event EventHandler<ServerEventArgs> EventImageRecieved;//回调

        public static void Start()
        {
            serverIP = IPAddress.Any;//定义自己监听的ip，可以是任何一个ip
            serverPort = 2020;//定义监听的端口
            server = new TcpListener(serverIP, serverPort);//建立TCP通讯
            binaryFormatter = new BinaryFormatter();//传输数据，序列化数据
            isOnline = true;//代表机器在线状态
            connectionTerminated = false;//表示连接是否终结
            server.Start();//连接开始
        }
        public static void Listen()
        {

            Task acceptClientTask = new Task(() => AcceptClient());//接收客户端连接
            acceptClientTask.Start();//开始接收
            acceptClientTask.Wait();//等待Task线程完成接收
            
            listeningTask = new Task(() => RecieveTransmission());//创建接收图片线程
            transmissionTask = new Task(() => SendTransmission());   //创建发送传输线程         
            listeningTask.Start();  //接收图片线程启动         
            transmissionTask.Start(); //发送传输线程启动          
            listeningTask.Wait();      //等待线程完成      
            transmissionTask.Wait();   //等待线程完成           
            Stop(new ServerErrorHandler("Client disconnected."));//异常输出

        }
        
        public static void AcceptClient()
        {
            client = server.AcceptTcpClient();//接收挂起的连接请求
            netStream = client.GetStream();//网络流
            binaryReader = new BinaryReader(netStream, Encoding.UTF8);//读流对象
            binaryWriter = new BinaryWriter(netStream, Encoding.UTF8);//写流对象
           
        }
        public static void Stop(IErrorLogger log)//错误回调
        {            
            if (connectionTerminated == false)//如果没终结
            {                
                try
                {                   
                    isOnline = false; //设置状态为离线                
                    binaryWriter.Close(); //关闭写流                  
                    binaryReader.Close();//关闭读流
                    netStream.Close();   //关闭网络流                 
                    client.Close();    //关闭客户端                
                    server.Stop();     //关闭服务端              
                    connectionTerminated = true;  //终结为真                 
                    if (parentForm != null)
                    {                        
                      parentForm.Invoke((MethodInvoker)delegate() { parentForm.Close(); });//如果出错，关闭主窗口
                    }
                    

                }
                catch (Exception ex)
                {
                    log.HandleException(ex);//异常日志
                }
            }
            

        }
        
        private static void RecieveTransmission()
        {
            if (client != null)//如果客户端存在
            {
                while (isOnline)//如果在线
                {
                    try
                    {
                        string message = binaryReader.ReadString();//开始收数据

                        switch (message)
                        {
                            case CommandImage:
                                RecieveImage();//如果收到是图片命令则开始收图
                                
                                break;
                            default:

                                break;

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                        isOnline = false;
                    }
                }

            }

        }
        private static void RecieveImage()//接收图像
        {
            Image screenshot = (Image)DesktopScreen.DeserializeScreen(netStream);//序列化屏幕截图
            OnEventImageRecieved(new ServerEventArgs() { Image = screenshot });//回调
        }
        public static void OnEventImageRecieved(ServerEventArgs args)
        {
            if (EventImageRecieved != null)//回调
            {
                EventImageRecieved(null, args);
            }
        }
        private static void SendTransmission()//发送传输
        { 

        }
    }
}
