using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    public static class Paths
    {
        static Paths() 
        {
            if (!Directory.Exists(SavePath)) 
                Directory.CreateDirectory(SavePath);
        }
        public static readonly string ServerPath = Path.Combine(Assembly.GetExecutingAssembly().Location, "..");
        public static readonly string SavePath = Path.Combine(ServerPath, "Saves");

    }
}
