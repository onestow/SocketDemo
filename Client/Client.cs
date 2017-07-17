using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Client
    {
        static Socket clientSocket;
        static void Main()
        {
            byte[] result = new byte[1024];
            //设定服务器IP地址  
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 1234)); //配置服务器IP与端口  
                Console.WriteLine("连接服务器成功");
            }
            catch
            {
                Console.WriteLine("连接服务器失败，请按回车键退出！");
                return;
            }

            //通过clientSocket接收数据  
            //int receiveLength = clientSocket.Receive(result);
            //Console.WriteLine("接收服务器消息：{0}", Encoding.UTF8.GetString(result, 0, receiveLength));
            //通过 clientSocket 发送数据  
            while(true)
            { 
                try
                {
                    //Thread.Sleep(10);
                    string sendMessage = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff");
                    clientSocket.Send(Encoding.UTF8.GetBytes(sendMessage));
                    Console.WriteLine("向服务器发送消息：" + sendMessage);
                }
                catch
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    break;
                }
            }
            Console.WriteLine("发送完毕，按回车键退出");
            Console.ReadLine();

        }

    }
}
