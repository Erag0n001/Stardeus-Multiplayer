using System;
using System.Net.Sockets;
using System.Text;
using Game.Misc;
using Multiplayer.Misc;
using Shared.Enums;
using Shared.Network;

namespace Multiplayer.Network
{
    public class ListenerClient : ListenerBase
    {
        public static ListenerClient Instance { get; private set; }
        public ListenerClient(TcpClient connection) : base(connection)
        {
            this.connection = connection;
            networkStream = connection.GetStream();
            Instance = this;
        }

        public override void HandlePacket(byte[] packetByte, PacketType packetType)
        {
            base.HandlePacket(packetByte, packetType);
            Printer.Warn(packetByte.Length);
            try
            {
                MainThread.Instance.Enqueue(() =>
                {
                    Main.Managers[packetType].Invoke(null, new object[] { packetByte });
                });
            }
            catch (Exception ex)
            {
                Printer.Error(ex);
            }
        }

        public override void HandleError(Exception exception, string method)
        {
            base.HandleError(exception, method);
            Printer.Error($"{method} in the TCP listener had exception:\n{exception}");
        }
        public override void HandleLogging(object obj)
        {
            Printer.Warn(obj);
        }
    }
}
