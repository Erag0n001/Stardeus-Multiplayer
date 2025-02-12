using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Shared.Enums;
using Shared.Misc;
using Shared.PacketData;

namespace Shared.Network
{
    public abstract class ListenerBase
    {
        public TcpClient connection;

        public NetworkStream networkStream;

        private readonly BlockingCollection<byte[]> dataQueue = new BlockingCollection<byte[]>();
        private bool disconnectFlag;
        public bool DisconnectFlag { get { return disconnectFlag; } 
            set 
            {
                if(value)
                    DestroyConnection();
            } 
        }

        private byte[] intBuffer = new byte[Serializer.SizeIdentifier];

        public ListenerBase(TcpClient connection)
        {
            this.connection = connection;
            networkStream = connection.GetStream();
        }

        public void EnqueueObject(PacketType header, object obj)
        {
            List<byte[]> packets = PacketUtility.CreatePacketsFromObject(header, obj);
            EnqueuePackets(packets);
        }

        public void EnqueuePackets(List<byte[]> packets)
        {
            if (disconnectFlag) return;
            foreach (byte[] packet in packets)
            {
                dataQueue.Add(packet);
            }
        }

        public void SendData()
        {
            try
            {
                while (!disconnectFlag)
                {
                    foreach (byte[] packet in dataQueue.GetConsumingEnumerable())
                    {
                        if (disconnectFlag) break;
                        networkStream.Write(packet, 0, packet.Length);
                    }
                }
            }

            catch (Exception e)
            {
                HandleError(e, nameof(SendData));

                disconnectFlag = true;
            }
        }

        public void Listen()
        {
            try
            {
                while (!disconnectFlag)
                {
                    if (!networkStream.DataAvailable || !networkStream.CanRead)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    networkStream.Read(intBuffer, 0, Serializer.SizeIdentifier); // We get the size of a packet
                    int packetBuffer = BitConverter.ToInt32(intBuffer, 0); //We get it in human readable numbers
                    Array.Clear(intBuffer, 0, intBuffer.Length);

                    byte[] headerBytes = new byte[Serializer.SizeForHeaderIdentifier];
                    networkStream.Read(headerBytes, 0, 1);
                    PacketType type = (PacketType)headerBytes[0];

                    byte[] packet = new byte[packetBuffer];
                    ReadFullPacket(packet, packetBuffer); // We get the entire object

                    HandlePacket(packet, type); //We do stuff with the objet bytes, crazy I know

                }
            }

            catch (Exception e)
            {
                HandleError(e, nameof(Listen));

                disconnectFlag = true;
            }
        }

        public void ReadFullPacket(byte[] content, int buffer)
        {
            int bytesRead = 0;
            try
            {
                while (bytesRead < buffer)
                {
                    int read = networkStream.Read(content, bytesRead, buffer - bytesRead);
                    if (read == 0) throw new Exception("Error while reading packet, no bytes left to read");
                    bytesRead += read;
                }
            }
            catch (Exception e)
            {
                HandleError(e, nameof(ReadFullPacket));
                disconnectFlag = true;
            }
        }
        public virtual void HandlePacket(byte[] packetByte, PacketType packetType)
        {

        }

        public void SendKAFlag()
        {
            try
            {
                while (!disconnectFlag)
                {
                    Thread.Sleep(1000);

                    List<byte[]> packet = PacketUtility.CreatePacketsFromObject(PacketType.KeepAlive, new KeepAliveData());
                    EnqueuePackets(packet);
                }
            }

            catch (Exception e)
            {
                HandleError(e, nameof(SendKAFlag));

                disconnectFlag = true;
            }
        }

        public virtual void HandleError(Exception exception, string method) { }
        public virtual void HandleLogging(object obj) { }
        public void DestroyConnection()
        {
            connection.Client.Shutdown(SocketShutdown.Both);

            connection.Close();

            disconnectFlag = true;

            HandleDisconnection();
        }
        public virtual void HandleDisconnection() { }
    }
}
