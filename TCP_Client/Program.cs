using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    private const int Port = 8888;
    private const string ServerIp = "127.0.0.1";
// задава стойностите на порта и IP адреса на сървъра към, който ще се свържем
    static void Main()
    {
        TcpClient client = new TcpClient(ServerIp, Port);
        Console.WriteLine("Connected to server. Start chatting!");
        //Създава TCP клиент

        NetworkStream stream = client.GetStream();

        // взема потока от данни, който се използва за комуникация между клиента и сървъра

        Thread receiveThread = new Thread(ReceiveMessages);
                //създава нов поток за получаване на съобщения от сървъра

        receiveThread.Start(stream);

        // стартира потока за получаване на съобщения


        while (true)
        {
            string message = Console.ReadLine();
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }
        // безкраен цикъл в, който програмата чете съобщенията въведени от потребителя в конзолата и ги изпраща на сървъра
    }

    static void ReceiveMessages(object obj)
    {
        NetworkStream stream = (NetworkStream)obj;
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
                Console.WriteLine(message);
            }
            catch (Exception)
            {
                break;
            }
        }
        // тази функция приема съобщенията от сървъра, криптира ги и ги изкарва на екрана. Ако потока се прекъсне или има грешка функцията излиза от цикъла и спира да приема съобщения.
    }
}
