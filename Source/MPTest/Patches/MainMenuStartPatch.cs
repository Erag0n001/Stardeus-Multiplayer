﻿using System;
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
        //TODO Fix this patch. Looks at ModCOnfig for info on how to.
        public static FieldInfo currentButtons;
        private static MethodInfo CreateButton;
        private static MethodInfo ShowMainMenu;
        public static bool WasPatched;
        
        static MainMenuStartPatch() 
        {
            currentButtons = AccessTools.Field(typeof(MainMenu), "currentButtons");
            CreateButton = AccessTools.Method(typeof(MainMenu), "CreateButton");
            ShowMainMenu = AccessTools.Method(typeof(MainMenu), "ShowMainMenu");
        }
        [HarmonyPatch(typeof(MainMenu), "ShowMainMenu")]
        public static class ShowMainMenuPatch 
        {
            [HarmonyPostfix]
            public static void Postfix(MainMenu __instance)
            {
                if (WasPatched)
                    return;
                List<MainMenuButton> buttons = (List<MainMenuButton>)currentButtons.GetValue(__instance);
                if (buttons == null)
                    return;
                buttons.Insert(3, CreateMPButton(__instance));
                WasPatched = true;
                ShowMainMenu.Invoke(__instance, null);   
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
