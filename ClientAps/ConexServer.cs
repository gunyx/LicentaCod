using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientAps
{
    class ConexServer
    {
        static int val_generala = 2; //folosita pentru "imitare counter descrescator"
        private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static object locker = new object();
        public ConexServer()
        {
            //clientSocketFct();
        }

        public void clientSocketFct()
        {
            Thread.Sleep(1000);
            LoopConnect();
            RequestLoop();
            //Console.ReadLine();
        }

        private static void LoopConnect()
        {
            int incercari = 0;

            while (!clientSocket.Connected)
            {
                try
                {
                    incercari++;
                    lock (locker)
                    {
                        clientSocket.Connect(IPAddress.Loopback, 4444);
                    }
                }
                catch (SocketException)
                {
                    Console.Clear();
                    Console.WriteLine("Incercari " + incercari.ToString());
                }

            }
            Console.Clear();
            Console.WriteLine("CONECTAT");
        }

        private static void RequestLoop()
        {
            firstSend();
            while (true)
            {
                SendRequest();
                ReceiveResponse();
                Console.WriteLine(val_generala.ToString());
                if (val_generala != -1)
                {
                    return;
                }
            }
        }

        private static void Exit()
        {
            SendString("exit");
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            Environment.Exit(0);
        }

        private static void firstSend()
        {
            string mesaj = "Accept";
            SendString(mesaj);
        }

        private static void SendRequest()
        {

            Console.Write("Scrie mesaj: ");
            string request = Console.ReadLine();
            SendString(request);

            if (request.ToLower() == "exit")
            {
                Exit();
            }
        }

        private static void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        private static string ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = clientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0)
            {
                return null;
            }
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);

            return text;
            // Console.WriteLine(text);
            /*
            if (text[0].Equals('R'))
            {
                string b = string.Empty;
                int val = 0;

                for (int i = 0; i < text.Length; i++)
                {
                    if (Char.IsDigit(text[i]))
                        b += text[i];
                }

                if (b.Length > 0)
                    val = int.Parse(b);

                val_generala = val;
            }else if( text[0].Equals('S'))
            {

            }*/

        }

        private static Boolean send_big_information(string filename)
        {
            using (var file = File.OpenRead(filename))
            {
                var sendBuffer = new byte[2048];
                var fileSize = BitConverter.GetBytes((int)file.Length);
                clientSocket.Send(fileSize, fileSize.Length, SocketFlags.None);//size file           
                byte[] fileName = Encoding.ASCII.GetBytes(filename);
                clientSocket.Send(fileName, fileName.Length, SocketFlags.None);//numele fisierului ptr. scrierea extensiei

                var bytesLeftToTransmit =fileSize.Length;
                while (bytesLeftToTransmit > 0)
                {
                    var dataToSend = file.Read(sendBuffer, 0, sendBuffer.Length);
                    bytesLeftToTransmit -= dataToSend;

                    var offset = 0;
                    while (dataToSend > 0)
                    {
                        var bytesSent = clientSocket.Send(sendBuffer, offset, dataToSend,SocketFlags.None);
                        dataToSend -= bytesSent;
                        offset += bytesSent;
                    }
                }
                return true;
            }
        }

        public Socket get_socket_client()
        {
            return clientSocket;
        }

    }
}