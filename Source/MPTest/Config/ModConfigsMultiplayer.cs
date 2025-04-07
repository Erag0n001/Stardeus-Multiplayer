using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModConfig.UI;
using static Multiplayer.Config.ModConfigsMultiplayer;

namespace Multiplayer.Config
{
    public class ModConfigsMultiplayer : ModConfigs
    {
        public override void DoWindowContent()
        {
            // Nothing for now
        }
    }
    public class Potato
    {
        public void FancyMethod() { }
    }

    public static class ExtensionOfWhatever
    {
        public static void FancierMethod(this Potato potato) { }
    }

    public class ActualCode 
    {
        public void DoStuff() 
        {
            Potato potato = new Potato();
            potato.FancierMethod(); //See how it wasn't in the base class?
        }
    }
}
