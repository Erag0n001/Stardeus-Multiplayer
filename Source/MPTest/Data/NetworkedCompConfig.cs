using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Commands;
using Game.Components;
using Game.Data;
using HarmonyLib;
using MessagePack;

namespace Multiplayer.Data
{
    [MessagePackObject]
    public class NetworkedCompConfig
    {
        [Key(0)] public string Component;
        [Key(1)] public List<byte[]> SerializableProperties = new List<byte[]>();

        public static NetworkedCompConfig GetFromConfig(ComponentConfig config) 
        {
            NetworkedCompConfig compConfig = new NetworkedCompConfig();
            compConfig.Component = config.Component;
            foreach (SerializableProperty property in config.Properties)
            {
                compConfig.SerializableProperties.Add(MessagePackSerializer.Serialize(property, CmdSaveGame.MsgPackOptions));
            }
            return compConfig;
        }

        public ComponentConfig ToConfig() 
        {
            ComponentConfig compConfig = new ComponentConfig();
            compConfig.Component = this.Component;
            foreach(byte[] value in this.SerializableProperties) 
            {
                compConfig.Properties.AddItem(MessagePackSerializer.Deserialize<SerializableProperty>(value));
            }
            return compConfig;
        }
    }
}
