using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace Multiplayer.Config
{
    public class ConfigDataMultiplayer : ModConfig.ConfigData
    {
        [Key(0)] public string previousIP = "127.0.0.1";
        [Key(1)] public int previousPort = 35000;
    }
}
