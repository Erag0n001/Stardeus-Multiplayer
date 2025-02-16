using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Game;
using Game.Commands;
using Game.Components;
using Game.Data;
using Game.Systems.Items;
using HarmonyLib;
using KL.Randomness;
using KL.Utils;
using MessagePack;
using Multiplayer.Data;
using Multiplayer.Patches;
using UnityEngine;
using static Game.UI.QuickSearch;

namespace Multiplayer.Misc
{
    public static class EntityWithCompsUtils
    {
        public static Entity Initialize(NetworkedEntityWithComp entityN)
        {
            GameStatePatch.AddEntityPatch.IsServer = true;

            Printer.Warn(entityN.data);
            Printer.Warn(entityN.type);
            Entity entity = (Entity)MessagePackSerializer.Deserialize(entityN.type, entityN.data, CmdSaveGame.MsgPackOptions);
            Def def = The.Defs.Get(entity.DefinitionId);
            Vector2 pos = new Vector2((float)entityN.x, (float)entityN.y);
            Printer.Warn($"Spawning entity {entity}");

            if (entityN.type == typeof(Obj))
            {
                ObjData data = A.S.Query.GenObjFull.Ask(def, Rng.Unseeded);
                entity = A.S.Objs.Create(pos, data, false);
            }
            else if (entityN.type == typeof(Tile))
            {
                EntityUtils.PlaceTile((Tile)entity, A.S.Grid.ToGridPos(new Vector2((float)entityN.x, (float)entityN.y)), false);
            }
            SetComps(entity, entityN);
            PostMake(entity, entityN);
            Printer.Warn($"Spawned entity {entity}");
            return entity;
        }

        private static void PostMake(Entity entity, NetworkedEntityWithComp entityN) 
        {
            if (entity is Obj)
            {
                Printer.Warn("Obj");
                NetworkedObj objN = (NetworkedObj)entityN;
                Obj obj = (Obj)entity;
                obj.Data.Quality = objN.quality ?? 0f;
                if(objN.color != null)
                    obj.Data.Color = objN.color.ToColor();
            }
            else if (entity is Being)
            {

            }
            Printer.Warn(entity.GetType());
        }
        public static void SetComps(Entity entity, NetworkedEntityWithComp objN) 
        {
            entity.Components = null;    
            for (int i = 0; i < objN.comps.Count; i++)
            {
                ComponentConfig config = objN.compConfigs[i].ToConfig();
                IComponent comp = The.Defs.CreateComponent(config, entity);
                entity.AddComponent(comp);

                ComponentData compData = MessagePackSerializer.Deserialize<ComponentData>(objN.comps[i], CmdSaveGame.MsgPackOptions);
                comp.Load(entity, compData);

                Printer.Warn(comp);
            }
        }

        public static void GetComps(Entity obj, NetworkedEntityWithComp networkedObject)
        {
            networkedObject.comps = new List<byte[]>();
            networkedObject.compConfigs = new List<NetworkedCompConfig>();
            foreach (IComponent comp in obj.Components)
            {
                comp.OnSave();
                ComponentData compData = comp.Data;

                byte[] data = MessagePackSerializer.Serialize(compData, CmdSaveGame.MsgPackOptions);
                Printer.Warn(comp);

                networkedObject.comps.Add(data);
                networkedObject.compConfigs.Add(NetworkedCompConfig.GetFromConfig(comp.Config));
            }
        }
    }
}
