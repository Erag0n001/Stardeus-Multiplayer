using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Components;
using Game.Systems.Items;
using MessagePack;
using UnityEngine;

namespace Multiplayer.Data
{
    [MessagePackObject]
    public class NetworkedEntityWithComp
    {
        [Key(0)] public int? id;
        [Key(1)] public byte[]? data;

        [Key(2)] public float? x;
        [Key(3)] public float? y;

        [Key(4)] private string? typestring { get; set; } //Apparently needed to serialize private fields
        [IgnoreMember] public Type type { 
            get 
            { 
                if(typestring != null)
                    return Type.GetType(typestring);
                else 
                    return null;
            }
            set 
            {
                typestring = value?.AssemblyQualifiedName;
            }
        }

        [Key(5)] public List<byte[]>? comps;
        [Key(6)] public List<NetworkedCompConfig>? compConfigs;
    }
}
