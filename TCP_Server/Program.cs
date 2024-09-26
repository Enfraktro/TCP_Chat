using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private static readonly List<TcpClient> clients = new List<TcpClient>();
    private const int Port = 8888;
     //Създава TCP сървър с порт 8888 от всеки IP адрес

    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, Port);
        server.Start();
        //Стартира сървъра и той започва да приема връзки от различни клиенти

        
        Console.WriteLine($"Server started on port {Port}");

       

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            clients.Add(client);
            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
// Създава нов поток за всеки новосвързан клиент, който активира функцията "Handle Client", която на свой ред отговаря за кореспонденцията с клиента

            
        }

        // в безкраен цикъл сървърът чака за връзка с клиент. Когато клиентът се свърже той го добавя към списъка с клиентите и се стартира нов поток, който обработва комуникацията.
    }

    static void HandleClient(object obj)
    {
        TcpClient tcpClient = (TcpClient)obj;
        NetworkStream stream = tcpClient.GetStream();

        byte[] buffer = new byte[1024];
        int bytesRead;
        while (true)
        {
            try
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");

                BroadcastMessage(tcpClient, message);
            }
            catch (Exception)
            {
                break;
            }
        }

        //Фунцкията обработва кореспонденцията с даден клиент.Чете съобщенията на клиентите и чрез фунцкията BroadcastMessage я препраща на всички други клиенти

        clients.Remove(tcpClient);
        tcpClient.Close();
    }

    static void BroadcastMessage(TcpClient sender, string message)
    {
        byte[] broadcastBuffer = Encoding.ASCII.GetBytes(message);

        foreach (TcpClient client in clients)
        {
            if (client != sender)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(broadcastBuffer, 0, broadcastBuffer.Length);
            }
        }
        // Изпраща на всички клиенти съобщенията освен на подателя на съобщението.
    }
}
