using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Shared.Enums;

namespace Shared.Misc
{

    public static class Serializer
    {
        public const int LeaderPacketSize = SizeIdentifier + SizeForHeaderIdentifier;

        public const int SizeIdentifier = 4;

        public const int SizeForHeaderIdentifier = 1;

        public const int MaxPacketSize = 65500;

        private static JsonSerializerSettings DefaultSettings => new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.None };
        private static JsonSerializerSettings ReadableSettings => new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.None
        };

        public static string ConvertObjectToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, ReadableSettings);
        }
        public static T ConvertJsonToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static byte[] ConvertObjectToBytes(object toConvert)
        {
            JsonSerializer serializer = JsonSerializer.Create(DefaultSettings);
            MemoryStream memoryStream = new MemoryStream();

            using (BsonWriter writer = new BsonWriter(memoryStream))
            {
                serializer.Serialize(writer, toConvert);
            }

            return memoryStream.ToArray();
        }

        public static T ConvertBytesToObject<T>(byte[] bytes)
        {
            JsonSerializer serializer = JsonSerializer.Create(DefaultSettings);
            MemoryStream memoryStream = new MemoryStream(bytes);

            using BsonReader reader = new BsonReader(memoryStream);
            return serializer.Deserialize<T>(reader);
        }

        public static List<byte[]> CreatePacketsFromObject(object serializable, PacketType header, bool shouldCompress = true)
        {
            byte[] objectInByte = ConvertObjectToBytes(serializable);
            return MakePacketsFromBytes(objectInByte, header, shouldCompress);
        }

        public static List<byte[]> MakePacketsFromBytes(byte[] bytes, PacketType header, bool shouldCompress) 
        {
            //if (shouldCompress)
            //{
            //    using (MemoryStream memoryStream = new MemoryStream())
            //    {
            //        using (GZipStream stream = new GZipStream(memoryStream, CompressionLevel.Optimal))
            //        {
            //            stream.Write(bytes, 0, bytes.Length);
            //        }
            //        bytes = memoryStream.ToArray();
            //    }
            //};
            List<byte[]> result = new List<byte[]>();
            byte[] sizeBuffer = new byte[LeaderPacketSize];
            Array.Copy(BitConverter.GetBytes(bytes.Length), 0, sizeBuffer, 0, SizeIdentifier);
            sizeBuffer[(SizeIdentifier + SizeForHeaderIdentifier) - 1] = (byte)header;
            result.Add(sizeBuffer);
            result.AddRange(SplitBytesIntoPackets(bytes));
            return result;
        }

        public static List<byte[]> SplitBytesIntoPackets(byte[] bytes) 
        {
            List<byte[]> result = new List<byte[]>();
            int writtenBytes = 0;
            while (writtenBytes < bytes.Length)
            {
                int packetSize = Math.Min(bytes.Length - writtenBytes, MaxPacketSize);

                byte[] packet = new byte[packetSize];
                Array.Copy(bytes, writtenBytes, packet, 0, packetSize);

                result.Add(packet);

                writtenBytes += packetSize;
            }
            return result;
        }

        public static T PacketToObject<T>(byte[] bytes)
        {
            //byte[] decompressedBytes;
            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    memoryStream.Write(bytes, 0, bytes.Length);
            //    memoryStream.Seek(0, SeekOrigin.Begin);
            //    using (GZipStream stream = new GZipStream(memoryStream, CompressionMode.Decompress))
            //    {
            //        using (MemoryStream decompressedStream = new MemoryStream())
            //        {
            //            stream.CopyTo(decompressedStream);
            //            decompressedBytes = decompressedStream.ToArray();
            //        }
            //    }
            //}
            return ConvertBytesToObject<T>(bytes);
        }

        public static void ObjectToFile(object obj, string path)
        {
            if (IsPathValid(path))
            {
                byte[] bytes = ConvertObjectToBytes(obj);
                File.WriteAllBytes(path, bytes);
            }
        }

        public static void ObjectToFileJson(object obj, string path, bool Json = true)
        {
            if (IsPathValid(path))
            {
                string json = JsonConvert.SerializeObject(obj);
                File.WriteAllText(path, json);
            }
        }

        public static T FileToObject<T>(string path, bool Json = true)
        {
            if (IsPathValid(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                return ConvertBytesToObject<T>(bytes);
            }
            return default;
        }

        private static bool IsPathValid(string path)
        {
            if (!string.IsNullOrEmpty(path) && path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
            {
                return false;
            }
            if (File.Exists(path))
            {
                return true;
            }
            else
            {
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                {
                    return true;
                }
            }
            return false;
        }
    }
}