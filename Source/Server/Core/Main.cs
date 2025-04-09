using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using Server.Managers;
using Server.Misc;
using Server.Network;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;

namespace Server.Core
{
    public static class MainProgram
    {
        public static Dictionary<PacketType, MethodInfo> Managers = new Dictionary<PacketType, MethodInfo>();
        public static UserClient host;
        static void Main(string[] args)
        {
            LoadAllManagers();
            NetworkHandler.StartListening();
            while (true)
            {
                Console.ReadLine();
            }
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
    }
}