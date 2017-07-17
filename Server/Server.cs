using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Server
    {
        static Socket serverSocket;
        public static ManualResetEvent TimeOutObject = new ManualResetEvent(false);
        static bool isRecSuc = false;
        static int len;
        static void Main()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
             serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
            serverSocket.Bind(new IPEndPoint(ip, 1234));  //绑定IP地址：端口  
            serverSocket.Listen(10);    //设定最多10个排队连接请求  
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());  
            //通过Clientsoket发送数据  
            Thread myThread = new Thread(ListenClientConnect);  
            myThread.Start();  
            Console.ReadLine();  

        }

        private static void ListenClientConnect()  
        {  
            while (true)  
            {  
                Socket clientSocket = serverSocket.Accept();  
                clientSocket.Send(Encoding.ASCII.GetBytes("Welcome"));  
                //Thread receiveThread = new Thread(ReceiveMessage);  
                Thread receiveThread = new Thread(RecMsgThread);  
                receiveThread.Start(clientSocket);  
            }  
        }



        private static void RecMsgThread(object obj)
        {
            Socket socket = obj as Socket;
            socket.BeginReceive(result, 0, 1024, SocketFlags.None, RecMsgCallBack, socket);
            while(true)
            {
                TimeOutObject.Reset();
                if(TimeOutObject.WaitOne(2000, false))
                {
                    if(isRecSuc)
                    {
                        Console.WriteLine(Encoding.ASCII.GetString(result, 0, len));
                    }
                    else
                    {
                        Console.WriteLine("disconnect");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("timeout");
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    return;
                }
            }
        }

        private static void RecMsgCallBack(IAsyncResult ar)
        {
            try
            {
                Socket socket = ar.AsyncState as Socket;
                len = socket.EndReceive(ar);
                if (len <= 0) isRecSuc = false;
                else
                {
                    //Console.WriteLine("************  " + Encoding.ASCII.GetString(result, 0, len));
                    isRecSuc = true;
                    socket.BeginReceive(result, 0, result.Length, SocketFlags.None, RecMsgCallBack, socket);
                }
            }
            catch (SocketException ex)
            {
                isRecSuc = false;
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                isRecSuc = false;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                TimeOutObject.Set();
            }
        }

        static byte[] result = new byte[1024];
    }
}
