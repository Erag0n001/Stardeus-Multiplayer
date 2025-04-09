using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Misc;
using Server.Network;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;

namespace Server.Managers
{
    public static class UserManager 
    {
        public static List<UserClient> ConnectedClients = new List<UserClient>();
        public static void SendClientPermissions(UserClient client) 
        {
            client.listener.EnqueueObject(PacketType.UserPermissions, client.permission);
        }
    }
    public static class UserHandler
    {
        [PacketHandler(PacketType.UserData)]
        public static void HandlePacket(UserClient client, byte[] packet)
        {
            UserData userData = Serializer.PacketToObject<UserData>(packet);
            client.AssignData(userData);
            Printer.Log($"New user registered {client.username}");
            UserManager.SendClientPermissions(client);
        }
    }
}
