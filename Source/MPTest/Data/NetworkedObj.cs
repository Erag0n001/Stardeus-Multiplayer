using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace Multiplayer.Data
{
    [MessagePackObject]
    public class NetworkedObj : NetworkedEntityWithComp
    {
        [Key(50)]public float? quality;
        [Key(51)]public NetworkedColor? color;
    }
}
