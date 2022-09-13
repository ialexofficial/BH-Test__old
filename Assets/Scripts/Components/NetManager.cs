using System;
using kcp2k;
using Mirror;

namespace Components
{
    public class NetManager : NetworkManager
    {
        protected KcpTransport _kcpTransport;

        public override void Start()
        {
            base.Start();

            _kcpTransport = GetComponent<KcpTransport>();
        }

        public void ConnectToServer(string host, ushort port, string nickname)
        {
            if (NetworkClient.active)
                throw new Exception("Already connected!");
            
            _kcpTransport.Port = port;
            networkAddress = host;
            StartClient();
        }

        public void StartHost(ushort port, string nickname)
        {
            if (NetworkClient.active)
                throw new Exception("Already connected!");
            
            _kcpTransport.Port = port;
            networkAddress = "localhost";
            StartHost();
        }

        public void DisconnectFromServer()
        {
            if (!NetworkClient.isConnected)
                return;
            
            if (NetworkServer.active)
            {
                StopHost();
            }
            else
            {
                StopClient();
            }
        }
    }
}