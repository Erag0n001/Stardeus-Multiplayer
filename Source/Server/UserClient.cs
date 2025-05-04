using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using Server.Core;
using Server.Managers;
using Server.Misc;
using Server.Network;
using Shared.PacketData;

namespace Server
{
    [MessagePackObject]
    public class UserClient
    {
        [IgnoreMember] public TcpClient client;
        [IgnoreMember] public ListenerServer listener;

        [Key(0)] public string username;
        [Key(1)] public UserPermission permission = new();
        [IgnoreMember] private bool hasBeenAssigned;
        public void AssignData(UserData data) 
        {
            if (hasBeenAssigned)
            {
                listener.DisconnectFlag = true;
                Printer.Error($"User {username ?? "null"} tried to initialize his data, even though it was already initialized. Disconnecting...");
                return;
            }

            username = data.username;
            if(UserManager.ConnectedClients.Count == 1) 
            {
                permission.isHost = true;
                Printer.Log($"{username} is now host of the game.");
                MainProgram.host = this;
            }
            hasBeenAssigned = true;
        }

        public override string ToString()
        {
            return username;
        }
    }
}
