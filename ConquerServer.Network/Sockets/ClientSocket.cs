using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ConquerServer.Network.Cryptography;
using System.Security.Cryptography;

namespace ConquerServer.Network.Sockets
{
    public class ClientSocket
    {
        public event Action<ClientSocket, Packet>? Message;
        public event Action<ClientSocket, Exception?>? Disconnected;

        private Socket? m_Socket;
        private byte[] m_Buffer;
        private bool m_Disconnected;
        public int HeaderSize { get; set; }
        public ICipher Cipher { get; private set; }
        public int ExpectedMessageSize { get; set; }
        public ServerSocket Server { get; private set; }
        public int Offset { get; private set; }
        public byte[] Padding { get; set; }

        public int Id { get; private set; }
        public object? State { get; set; }
        
        // no padding used on auth server, so if there is padding we're on the game server
        private bool IsGameServerClient {  get { return Padding.Length > 0; } }

        public LinkedList<byte[]> NetworkChunks { get; private set; }

        public ClientSocket(int id, Socket clientSocket, ServerSocket server, ICipher cipher)
        {
            Id = id;
            Cipher = cipher;
            m_Socket = clientSocket;
            Server = server;
            m_Buffer = new byte[0];
            Padding = new byte[0];
            NetworkChunks = new LinkedList<byte[]>();

            Initialize();
        }

        public void Send(Packet p)
        {
            byte[] msg = new byte[p.Size];
            p.CopyTo(msg);
            Send(msg);
        }

        public void Send(byte[] p)
        {
            if (m_Socket == null)
                throw new InvalidOperationException("Socket cannot be null");

            byte[] enc = new byte[p.Length + Padding.Length];
            p.CopyTo(enc, 0);
            Array.Copy(Padding, 0, enc, p.Length, Padding.Length);
            Cipher.Encrypt(enc, 0, enc, 0, enc.Length);

            m_Socket.BeginSend(enc, 0, enc.Length, SocketFlags.None, (res) =>
            {
                try
                {
                    m_Socket.EndSend(res);
                }
                catch (SocketException)
                {
                    Disconnect();
                }

            }, null);
        }

        public void Disconnect()
        {
            if (m_Disconnected) 
                return;

            try
            {
                if (m_Socket == null)
                    return;

                m_Disconnected = true;
                m_Socket.Disconnect(false);
            }
            catch (SocketException)
            {
                //
            }


            if (Disconnected != null)
                Disconnected(this, null);
        }

        private void Initialize()
        {
            m_Buffer = new byte[1024];
            HeaderSize = 2;
            ExpectedMessageSize = 2;
            Offset = 0;
        }

        public void StartReceive()
        {
            if (m_Socket != null)
            {
                m_Socket.BeginReceive(
                    m_Buffer,
                    Offset,
                    ExpectedMessageSize,
                    SocketFlags.None,
                    Receive,
                    null);
            }
        }

        private void Receive(IAsyncResult res)
        {
            if (m_Socket == null)
                throw new InvalidOperationException("Cannot receive m_Socket is null");

            int messageLength;
            try
            {
                messageLength = m_Socket.EndReceive(res);
                if (messageLength == 0)
                {
                    Disconnect();
                    return;
                }

                ExpectedMessageSize -= messageLength;
                Offset += messageLength;

                if (ExpectedMessageSize == 0)
                {
                    if (Offset == HeaderSize)
                    {
                        Cipher.Decrypt(m_Buffer, 0, m_Buffer, 0, Offset);

                        ushort size = BitConverter.ToUInt16(m_Buffer, HeaderSize - 2);
                        ExpectedMessageSize = size - Offset + Padding.Length + (HeaderSize > 2 ? (-1) : 0);
                    }
                    else
                    {
                        Cipher.Decrypt(m_Buffer, HeaderSize, m_Buffer, HeaderSize, Offset - HeaderSize);

                        // dispatch the packet without the padding
                        var p = new Packet(m_Buffer, Offset - Padding.Length);
                        if (Message != null)
                            Message(this, p);

                        Offset = 0;
                        ExpectedMessageSize = HeaderSize;
                    }
                }
            }
            catch (SocketException ex)
            {
                Disconnect();
                return;
            }

            StartReceive();
        }
    }
}