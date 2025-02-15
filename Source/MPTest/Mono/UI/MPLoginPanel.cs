using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Systems.Twitch;
using Game.UI;
using Multiplayer.Misc;
using Multiplayer.Network;
using Shared.Enums;
using Shared.PacketData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Multiplayer.Patches.MainMenuStartPatch;

namespace Multiplayer.Mono.UI
{
    public class MPLoginPanel : MonoBehaviour, IUIPanel
    {
        private TMP_InputField ipInput;

        private TMP_InputField portInput;
        public void SetActive(bool on)
        {
            base.gameObject.SetActive(on);
            if (on)
                Printer.Warn("Save the login data");
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Register()
        {
            Panels.AddSettingsPanel("menu.multiplayer.login", new PanelDescriptor(typeof(MPLoginPanel), isGlobal: true, -2147483643, "Icons/White/Twitch", skipInGame: true));
        }
        private void Start()
        {
            UIBuilder.CreateText("UIHeadingWidget", "Multiplayer", base.transform);
            UIBuilder.CreateText("UILabelWidget", "IP:", base.transform);
            ipInput = UIBuilder.CreateInputField("UITextInputWidget", "127.0.0.1", "ip here", base.transform);
            UIBuilder.CreateText("UILabelWidget", "Port:", base.transform);
            portInput = UIBuilder.CreateInputField("UITextInputWidget", "35000", "port here", base.transform);
            UIBuilder.CreateButton(
                "UIMenuButtonWidget",
                "Connect",
                () => { Connect(); },
                base.transform
            );
        }
        private void Connect() 
        {
            try
            {
                string ip = ipInput.text;
                if (int.TryParse(portInput.text, out int port))
                {
                    NetworkHandler.CreateConnection(ip, port);
                    ListenerClient.Instance.EnqueueObject(PacketType.RequestSaveFile, null);
                }
                else
                {
                    Printer.Warn("Invalid port");
                }
            }
            catch (Exception e)
            {
                Printer.Error($"Error while connecting:{e.ToString()}");
            }
        }
    }
}
