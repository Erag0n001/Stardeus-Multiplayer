using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Commands;
using Game.Utils;
using KL.Utils;
using Multiplayer.Misc;
using Multiplayer.Patches;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Multiplayer.Managers
{
    public static class SaveManager
    {
        public static string SaveFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
            "..",
            "LocalLow",
            "Kodo Linija",
            "Stardeus",
            "MpSaves"
            );
        public static readonly string SaveExtension=  ".save";
        public static void LoadGameWithoutMeta(SaveFile save) 
        {
            if (save.name == null)
            {
                Printer.Error("Save file from server was null.");
                return;
            }
            CmdLoadGamePatch.IsFromServer = true;
            string folderPath = Path.Combine(SaveManager.SaveFolder, save.name);
            string savePath = Path.Combine(folderPath, save.name);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            File.WriteAllBytes(savePath + SaveManager.SaveExtension, save.data);

            The.BB.Set(-1255412797, "load_game");
            The.BB.Set(303967644, savePath);
            The.BB.Set(-2080448491, save.name);

            D.Warn("Loading game: {0}", save.name);
            MiscUtils.LoadSceneTimed("Empty");
            MiscUtils.LoadSceneTimed("Game");
            SaveLoadUtils.ClearCache();
        }
    }
    public static class SaveHandler 
    {
        [PacketHandler(PacketType.SaveFileSend)]
        public static void HandleSaveFile(byte[] packet) 
        {
            SaveFile save = Serializer.PacketToObject<SaveFile>(packet);
            SaveManager.LoadGameWithoutMeta(save);
        }
    }
}
