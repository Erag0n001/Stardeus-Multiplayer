using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Core;
using Server.Managers;
using Shared.PacketData;

namespace Server.Data
{
    public class PendingGoal
    {
        public PendingGoal(NetworkedAIGoal goal) 
        {
            this.goal = goal;
            acknowlegedClients = new();
        }
        public NetworkedAIGoal goal;
        public HashSet<UserClient> acknowlegedClients;
    }
}
