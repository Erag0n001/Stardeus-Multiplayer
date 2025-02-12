using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Core;
using Server.Misc;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;

namespace Server.Managers
{
    public static class SaveManager
    {
        public static byte[]? GetSaveFileFromUsername(string username)
        {
            foreach(string file in Directory.GetFiles(Paths.SavePath)) 
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if(name == username)
                    return File.ReadAllBytes(file);
            }
            return null;
        }
    }
    public static class SaveHandler
    {
        [PacketHandler(PacketType.SaveFileSend)]
        public static void HandleSaveFile(UserClient client, byte[] packet)
        {
            SaveFile file = Serializer.PacketToObject<SaveFile>(packet);
            string path = Path.Combine(Paths.SavePath, MainProgram.host.username + ".mpSave");
            if (File.Exists(path))
                File.Delete(path);
            File.WriteAllBytes(path, file.data);
        }
        [PacketHandler(PacketType.RequestSaveFile)]
        public static void HandleSaveFileRequest(UserClient client, byte[] packet)
        {
            SaveFile file = new SaveFile();
            if (client.permission.isHost) 
            {
                byte[] data = SaveManager.GetSaveFileFromUsername(client.username);
                if (data == null)
                {
                    file.name = null;
                }
                else
                {
                    file.data = data;
                    file.name = client.username;
                }
            }
            else 
            {
                byte[] data = SaveManager.GetSaveFileFromUsername(MainProgram.host.username);
                if(data == null)
                {
                    Printer.Error("Host has null save file. This should not be possible.");
                    return;
                }
                file.name = MainProgram.host.username;
                file.data = data;
            }
            client.listener.EnqueueObject(PacketType.SaveFileSend, file);
        }
    }
}
