using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Multiplayer.Misc;
using Multiplayer.Network;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;

namespace Multiplayer.Managers
{
    public static class UserManager 
    {

    }
    public static class UserHandler
    {
        [PacketHandler(PacketType.UserPermissions)]
        public static void HandleClientPermissions(byte[] packet) 
        {
            UserPermission perms = Serializer.PacketToObject<UserPermission>(packet);
            Main.isHost = perms.isHost;
            Printer.Warn($"Welcome {The.Platform.PlayerName}, you are {(perms.isHost ? "the host":"not the host")}.");
            if (!perms.isHost)
            {
                Printer.Warn($"Awaiting save file...");
            }
        }
        [PacketHandler(PacketType.DisconnectSafe)]
        public static void DisconnectFromServer(byte[] packet)
        {
            ListenerClient.Instance.DisconnectFlag = true;
        }
    }
}
