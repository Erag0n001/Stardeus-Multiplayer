using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Server.Misc;
namespace Server.Network
{
    public static class NetworkHandler
    {
        public static string ip = "0.0.0.0";
        public static int port = 35000;
        public static TcpListener server;
        public static void CreateConnection(TcpClient client)
        {
            ListenerServer listener = new ListenerServer(client);
            Task.Run(() => { listener.Listen(); });
            Task.Run(() => { listener.SendData(); });
            Task.Run(() => { listener.SendKAFlag(); });
        }

        public static void ListenForConnections()
        {
            TcpClient newTCP = server.AcceptTcpClient();
            CreateConnection(newTCP);
            Printer.Log($"Made connection");
        }

        public static void StartListening()
        {
            server = new TcpListener(IPAddress.Parse(ip), port);
            server.Start();
            Task.Run(() =>
            {
                while (true) { ListenForConnections(); }
            });
            Printer.Log($"Started listening on {ip}:{port}");
        }
    }
}