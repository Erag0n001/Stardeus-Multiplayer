using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Server.Core;
using Server.Misc;
using Server.Network;
using Shared.PacketData;

namespace Server
{
    public class UserClient
    {
        public TcpClient client;
        public ListenerServer listener;

        public string username;
        public UserPermission permission = new();
        [JsonIgnore] private bool hasBeenAssigned;
        public void AssignData(UserData data) 
        {
            if (hasBeenAssigned)
            {
                listener.DisconnectFlag = true;
                Printer.Error($"User {username ?? "null"} tried to initialize his data, even though it was already initialized. Disconnecting...");
                return;
            }

            username = data.username;
            if(MainProgram.Users.Count == 1) 
            {
                permission.isHost = true;
                Printer.Log($"{username} is now host of the game.");
                MainProgram.host = this;
            }
            hasBeenAssigned = true;
        }
    }
}
