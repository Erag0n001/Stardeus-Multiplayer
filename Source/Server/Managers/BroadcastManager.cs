using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Core;
using Server.Misc;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;
namespace Server.Managers
{
    public static class BroadcastManager
    {
        public static void BroadcastToAllClient(UserClient client, byte[] data, PacketType header, bool excludeSelf = true, bool exludeHost = true)
        {
            foreach (UserClient cl in UserManager.ConnectedClients)
            {
                if ((excludeSelf && cl == client) || exludeHost && cl == MainProgram.host) continue;
                cl.listener.EnqueuePackets(Serializer.MakePacketsFromBytes(data, header, false));
            }
        }
        public static void BroadcastToAllClient(UserClient client, List<byte[]> packets, bool excludeSelf = true, bool exludeHost = true)
        {
            foreach (UserClient cl in UserManager.ConnectedClients)
            {
                if ((excludeSelf && cl == client) || exludeHost && cl == MainProgram.host) continue;
                cl.listener.EnqueuePackets(packets);
            }
        }
    }
    public static class BroadcastHandler 
    {
        // Entity related
        [PacketHandler(PacketType.BroadCastNewObj)]
        public static void BroadCastObject(UserClient client, byte[] packet)
        {
            BroadcastManager.BroadcastToAllClient(client, packet, PacketType.BroadCastNewObj);
        }
        [PacketHandler(PacketType.BroadCastNewEntity)]
        public static void BroadCastNewEntity(UserClient client, byte[] packet)
        {
            BroadcastManager.BroadcastToAllClient(client, packet, PacketType.BroadCastNewEntity);
        }
        [PacketHandler(PacketType.BroadCastEntityDelete)]
        public static void BroadCastEntityDelete(UserClient client, byte[] packet)
        {
            BroadcastManager.BroadcastToAllClient(client, packet, PacketType.BroadCastEntityDelete);
        }


        [PacketHandler(PacketType.MousePos)]
        public static void HandleMousePos(UserClient client, byte[] packet)
        {
            MouseData data = Serializer.PacketToObject<MouseData>(packet);
            data.username = client.username;
            List<byte[]> packets = Serializer.CreatePacketsFromObject(data, PacketType.MousePos);
            BroadcastManager.BroadcastToAllClient(client, packets);
        }
    }
}
