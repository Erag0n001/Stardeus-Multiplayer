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
    public class NetworkedEntityWithComp
    {
        public int? id;
        public byte[]? data;

        public float? x;
        public float? y;

        public Type? type;

        public List<byte[]>? comps;
        public List<NetworkedCompConfig>? compConfigs;
    }
}
