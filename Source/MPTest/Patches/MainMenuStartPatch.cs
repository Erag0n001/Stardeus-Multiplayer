using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Systems.Twitch;
using Game.UI;
using HarmonyLib;
using KL.Clock;
using Multiplayer.Misc;
using Multiplayer.Mono.UI;
using TMPro;

namespace Multiplayer.Patches
{
    public static class MainMenuStartPatch
    {
        public static FieldInfo currentButtons;
        private static FieldInfo text;
        private static MethodInfo CreateButton;
        private static MethodInfo ShowMainMenu;
        private static bool wasPatched;
        
        static MainMenuStartPatch() 
        {
            currentButtons = AccessTools.Field(typeof(MainMenu), "currentButtons");
            text = AccessTools.Field(typeof(MainMenuButton), "text");
            CreateButton = AccessTools.Method(typeof(MainMenu), "CreateButton");
            ShowMainMenu = AccessTools.Method(typeof(MainMenu), "ShowMainMenu");
        }
        [HarmonyPatch(typeof(MainMenu), "Update")]
        public static class StartPatch 
        {
            [HarmonyPostfix]
            public static void Postfix(MainMenu __instance)
            {
                if (wasPatched)
                    return;
                List<MainMenuButton> buttons = (List<MainMenuButton>)currentButtons.GetValue(__instance);
                if (buttons == null)
                    return;
                buttons.Insert(3, CreateMPButton(__instance));
                ShowMainMenu.Invoke(__instance, null);   
                wasPatched = true;
            }

            private static MainMenuButton CreateMPButton(MainMenu __instance)
            {
                Action<MainMenuButton> action = delegate { CreateMPPannel(__instance); };
                MainMenuButton button = (MainMenuButton)CreateButton.Invoke(__instance, new object[] { "Multiplayer" , action , null} );
                return button;
            }
            private static void CreateMPPannel(MainMenu __instance)
            {
                The.SysSig.ShowPanel.Send(new PanelDescriptor(typeof(MPLoginPanel), withCloseButton: true, skipInGame: true));
            }
        }
    }
}
