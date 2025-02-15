using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Data
{
    public class NetworkedObj : NetworkedEntityWithComp
    {
        public float? quality;
        public NetworkedColor? color;
    }
}
