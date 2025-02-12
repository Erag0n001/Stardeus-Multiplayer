using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Game;
using Game.Platform;
using Multiplayer.Managers;
using Multiplayer.Misc;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;

namespace Multiplayer.Network
{
    public static class NetworkHandler
    {
        public static ListenerClient listener;
        public static void CreateConnection(string ip, int port)
        {
            Printer.Warn($"Established connection with {ip}:{port}");
            ListenerClient listener = new ListenerClient(new TcpClient(ip, port));
            Task.Run(() => { listener.Listen(); });
            Task.Run(() => { listener.SendData(); });
            Task.Run(() => { listener.SendKAFlag(); });
            UserData data = new UserData();
            data.username = The.Platform.PlayerName ?? "Test";
            ListenerClient.Instance.EnqueueObject(PacketType.UserData, data);
            Main.isConnected = true;
        }
    }
}