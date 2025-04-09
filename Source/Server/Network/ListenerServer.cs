using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;
using Server.Core;
using Server.Managers;
using Server.Misc;
using Shared.Enums;
using Shared.Misc;
using Shared.Network;

namespace Server.Network
{
    public class ListenerServer : ListenerBase
    {
        public static ListenerServer Instance { get; private set; }
        public UserClient UserClient { get; private set; }
        public ListenerServer(TcpClient connection) : base(connection)
        {
            this.connection = connection;
            networkStream = connection.GetStream();
            Instance = this;
            UserClient = new UserClient();
            UserClient.client = this.connection;
            UserClient.listener = this;
            UserManager.ConnectedClients.Add(UserClient);
        }

        public override void HandlePacket(byte[] packetByte, PacketType packetType)
        {
            base.HandlePacket(packetByte, packetType);
            if(packetType != PacketType.KeepAlive)
                Printer.Warn($"{DateTime.UtcNow:HH:mm:ss:} Packet arrived with header {packetType} from {UserClient.username} and size {packetByte.Length + Serializer.LeaderPacketSize}");
            try
            {
                if (!MainProgram.Managers.ContainsKey(packetType)) 
                {
                    Printer.Error($"{packetType.ToString()} was not present in the manager list");
                }
                MainProgram.Managers[packetType].Invoke(null, new object[] { UserClient, packetByte });
            }
            catch (Exception ex)
            {
                Printer.Error(ex);
            }
        }

        public override void HandleError(Exception exception, string method)
        {
            Printer.Error($"{method} in the TCP listener had exception:\n{exception}");
        }

        public override void HandleLogging(object obj)
        {
            Printer.Log(obj);
        }

    }
}
