using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace Shared.PacketData
{
    [MessagePackObject]
    public class NetworkedAIGoal
    {
        public NetworkedAIGoal(uint goalId, int agentId, byte[] goal, byte[] plan)
        {
            goalID = goalId;
            agentID = agentId;
            this.goal = goal;
            this.plan = plan;
        }
        [Key(0)] public uint goalID;
        [Key(1)] public int agentID;
        [Key(2)] public byte[] goal;
        [Key(3)] public byte[] plan;
    }
}
