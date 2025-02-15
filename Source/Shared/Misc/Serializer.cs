using MessagePack;
using Shared.Enums;

namespace Shared.Misc
{

    public static class Serializer
    {
        public const int LeaderPacketSize = SizeIdentifier + SizeForHeaderIdentifier;

        public const int SizeIdentifier = 4;

        public const int SizeForHeaderIdentifier = 1;

        public const int MaxPacketSize = 65500;

        private static MessagePackSerializerOptions DefaultMessagePackOptions =>
            MessagePackSerializerOptions.Standard
                .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);
        private static MessagePackSerializerOptions ReadableMessagePackOptions =>
            MessagePackSerializerOptions.Standard
                .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance)
                .WithCompression(MessagePackCompression.None)
                .WithAllowAssemblyVersionMismatch(true);

        public static string ConvertObjectToJson(object obj)
        {
            byte[] data = MessagePackSerializer.Serialize(obj, ReadableMessagePackOptions);
            return MessagePackSerializer.ConvertToJson(data, ReadableMessagePackOptions);
        }

        public static T ConvertJsonToObject<T>(string json)
        {
            byte[] data = MessagePackSerializer.ConvertFromJson(json);
            return MessagePackSerializer.Deserialize<T>(data);
        }

        public static byte[] ConvertObjectToBytes(object toConvert)
        {
            return MessagePackSerializer.Serialize(toConvert, DefaultMessagePackOptions);
        }

        public static T ConvertBytesToObject<T>(byte[] bytes)
        {
            return MessagePackSerializer.Deserialize<T>(bytes, DefaultMessagePackOptions);
        }

        public static List<byte[]> CreatePacketsFromObject(object serializable, PacketType header, bool shouldCompress = true)
        {
            byte[] objectInByte;
            if (serializable != null)
                objectInByte = ConvertObjectToBytes(serializable);
            else
                objectInByte = new byte[0];
            return MakePacketsFromBytes(objectInByte, header, shouldCompress);
        }

        public static List<byte[]> MakePacketsFromBytes(byte[] bytes, PacketType header, bool shouldCompress) 
        {
            // TODO, find better way to compress packet, current implementation lowers performance

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
            result.Add(CreateLeaderPacket(bytes, header));
            if(bytes.Length > 0)
                result.AddRange(SplitBytesIntoPackets(bytes));
            return result;
        }

        private static byte[] CreateLeaderPacket(byte[] bytes, PacketType header) 
        {
            byte[] sizeBuffer = new byte[LeaderPacketSize];
            Array.Copy(BitConverter.GetBytes(bytes.Length), 0, sizeBuffer, 0, SizeIdentifier);
            sizeBuffer[(SizeIdentifier + SizeForHeaderIdentifier) - 1] = (byte)header;
            return sizeBuffer;
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
            // TODO, find better way to compress packet, current implementation lowers performance

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
                string json = ConvertObjectToJson(obj);
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