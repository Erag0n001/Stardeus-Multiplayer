using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace Multiplayer.Data
{
    [MessagePackObject]
    public class NetworkedAIGoal
    {
        [Key(0)] public int? entityID;
        [Key(1)] public byte[] goal;
    }
}
