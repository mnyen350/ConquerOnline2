using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using ConquerServer.Network.Cryptography;

namespace ConquerServer.Network.Sockets
{
    public abstract class ServerSocket
    {
        public event Action<ClientSocket>? ClientConnected;
        public event Action<ClientSocket>? ClientDisconnected;
        public event Action<ClientSocket, Packet>? ClientMessage;

        private Socket m_Socket;
        private int m_NextClientId;
        private ConcurrentDictionary<int, ClientSocket> m_Clients;

        public ClientSocket[] Clients { get { return m_Clients.Values.ToArray(); } }

        public ServerSocket(int port)
        {
            m_Clients = new ConcurrentDictionary<int, ClientSocket>();
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_Socket.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        protected abstract ICipher CreateCipher();
        protected virtual bool OnClientConnected(ClientSocket client) { return true; }
        protected virtual bool OnClientMessage(ClientSocket client, Packet message) { return true; }

        protected void RaiseClientConnected(ClientSocket client)
        {
            ClientConnected?.Invoke(client);
        }

        protected void RaiseClientMessage(ClientSocket client, Packet message)
        {
            ClientMessage?.Invoke(client, message);
        }

        protected void RaiseClientDisconnected(ClientSocket client)
        {
            ClientDisconnected?.Invoke(client);
        }

        public void Start()
        {
            m_Socket.Listen(100);
            m_Socket.BeginAccept(AcceptClient, null);
        }

        private void AcceptClient(IAsyncResult res)
        {
            try
            {
                var client = new ClientSocket(++m_NextClientId, m_Socket.EndAccept(res), this, CreateCipher());
                m_Clients[client.Id] = client;

                client.Message += (s, message) =>
                { 
                    if (OnClientMessage(client, message))
                        RaiseClientMessage(client, message);
                };
                client.Disconnected += (s, e) =>
                {
                    ClientSocket dummy;
                    m_Clients.TryRemove(client.Id, out dummy);
                    RaiseClientDisconnected(client);
                };

                if (OnClientConnected(client))
                    RaiseClientConnected(client);

                client.StartReceive();
            }
            catch (SocketException)
            {
                // do nothing
            }

            m_Socket.BeginAccept(AcceptClient, null);
        }
        public void Stop()
        {
            m_Socket.Close();
        }
    }
}