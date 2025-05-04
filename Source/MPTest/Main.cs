using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KL.Utils;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using Shared.Misc;
using Multiplayer.Misc;
using Multiplayer.Network;
using Multiplayer.Mono.UI;
using Game;
using Game.Misc;
using Shared.Enums;
using Multiplayer.Config;
using ModConfig;
using ModConfig.Patches;
using UnityEngine.SceneManagement;
using Multiplayer.Patches;

namespace Multiplayer
{
    public static class Main
    {
        public static Harmony harmony;
        public static Dictionary<PacketType, MethodInfo> Managers = new Dictionary<PacketType, MethodInfo>();
        public static bool isHost = false;
        public static bool isConnected = false;
        public static ConfigDataMultiplayer Configs;
        [RuntimeInitializeOnLoadMethod]
        static void StaticConstructorOnStartup() 
        {
            Configs = (ConfigDataMultiplayer)ConfigData.LoadConfig("Eragon.Multiplayer");
            Printer.Warn("Multiplayer Loaded!");
            LoadHarmony();
            SetupListeners();
            CreateUnityDispatcher();
            Task.Run(() =>
            {
                LoadAllManagers();
            });
        }

        static void LoadHarmony() 
        {
            harmony = new Harmony("Eragon.Multiplayer");
            harmony.PatchAll();
        }

        public static void LoadAllManagers()
        {
            foreach (MethodInfo method in Assembly.GetExecutingAssembly().
                GetTypes().
                SelectMany(t => t.GetMethods()).
                Where(m => m.GetCustomAttribute<PacketHandlerAttribute>() != null))
            {
                PacketHandlerAttribute attribute = method.GetCustomAttribute<PacketHandlerAttribute>();
                try
                {
                    if (!method.IsStatic)
                    {
                        Printer.Error($"Tried to add {attribute.Header} as a PacketHandler, but {method.Name} was not static");
                        continue;
                    }
                    if (Managers.Keys.Contains(attribute.Header))
                    {
                        Printer.Error($"Tried to add {attribute.Header} as a PacketHandler, but {attribute.Header} is already in use");
                        continue;
                    }
                    Managers[attribute.Header] = method;
                }
                catch (Exception exception) { Console.WriteLine($"{attribute.Header} failed to load\n{exception}"); }
            }
        }

        private static void CreateUnityDispatcher()
        {
            GameObject go = new GameObject("Dispatcher");
            go.AddComponent(typeof(MainThread));
        }

        static void SetupListeners()
        {
            SceneManager.activeSceneChanged += ListenForSceneChange;
        }

        static void ListenForSceneChange(Scene before, Scene after)
        {
            if (after.name == "MainMenu")
                MainMenuStartPatch.WasPatched = false;
        }
    }
}
