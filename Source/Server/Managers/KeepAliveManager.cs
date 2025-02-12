using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Misc;
using Shared.Enums;
using Shared.Misc;

namespace Server.Managers
{
    public static class KeepAliveManager
    {
        [PacketHandler(PacketType.KeepAlive)]
        public static void HandlePacket(UserClient client ,byte[] packet)
        {

        }
    }
}
