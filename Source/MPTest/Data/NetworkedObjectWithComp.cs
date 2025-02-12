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
        public int? id;
        public byte[]? data;

        public int? posIdx;

        public Type? type;

        public List<byte[]>? comps = new List<byte[]>();
        public List<string>? compDefinitions = new List<string>();
    }
}
