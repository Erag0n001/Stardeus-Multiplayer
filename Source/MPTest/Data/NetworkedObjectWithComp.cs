using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Components;
using Game.Systems.Items;
using MessagePack;
using Newtonsoft.Json;
using UnityEngine;

namespace Multiplayer.Data
{
    public class NetworkedObjectWithComp
    {
        public Type? type;
        public Dictionary<Type, byte[]> comps = new Dictionary<Type, byte[]>();
    }
}
