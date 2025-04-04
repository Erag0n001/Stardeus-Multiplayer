using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Game;
using Game.Commands;
using Game.Data;
using Game.Rendering;
using Game.Utils;
using Multiplayer.Data;
using Multiplayer.Misc;
using Multiplayer.Mono.GameObjects;
using Multiplayer.Network;
using Multiplayer.Patches;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;
using UnityEngine;

namespace Multiplayer.Managers
{
    public static class InputManager
    {
        public const float sleepTime = 25;
        private static Dictionary<string, Mouse> mouses = new Dictionary<string, Mouse>();
        public static void StartBroadcastingMousePos() 
        {
            Task.Run(() =>
            {
                Printer.Warn("Started broadcasting mouse");
                while (true)
                {
                    Thread.Sleep((int)sleepTime);
                    try { BroadcastMousePosition(); }
                    catch(Exception e) { Printer.Error(e); }
                }
            });
        }
        private static void BroadcastMousePosition() 
        {
            MouseData data = new MouseData();
            Vector2 mousePos = The.Bindings.Mouse.WorldPosition;
            if (mousePos == null)
                return;
            data.x = mousePos.x;
            data.y = mousePos.y;
            ListenerClient.Instance.EnqueueObject(PacketType.MousePos, data);
        }

        public static void MoveCursorFromUser(MouseData data) 
        {
            if (!Ready.Game)
                return;
            if (!mouses.ContainsKey(data.username))
                CreateMouseGameobject(data);
            Mouse mouse = mouses[data.username];
            mouse.SetLerpData(data);
        }

        private static GameObject CreateMouseGameobject(MouseData data) 
        {
            Printer.Warn($"Creating mouse for {data.username}...");

            GameObject gameObject = new GameObject($"Mouse_{data.username}");
            //GameObject instance = UnityEngine.Object.Instantiate(gameObject);

            SpriteRenderer renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = Images.CursorSprite("Cursor");
            renderer.sortingOrder = 2;
            renderer.color = new Color(1, 1, 1, 1);
            renderer.material = new Material(Shader.Find("Sprites/Default"));
            renderer.renderingLayerMask = 1;

            if (renderer.sprite == null)
                Printer.Warn("Cursor sprite is missing!");

            Mouse mouse = gameObject.AddComponent<Mouse>();

            gameObject.transform.position = new Vector3(data.x, data.y, 300);
            gameObject.transform.localScale = Vector3.one;
            gameObject.layer = LayerMask.NameToLayer("ObjectsAbove");

            mouses.Add(data.username, mouse);
            return gameObject;
        }
    }
    public static class InputHandler 
    {
        [PacketHandler(PacketType.MousePos)]
        public static void MoveMouseFromPacket(byte[] packet)
        {
            MouseData data = Serializer.PacketToObject<MouseData>(packet);
            InputManager.MoveCursorFromUser(data);
        }
    }
}
