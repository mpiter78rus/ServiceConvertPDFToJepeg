using System.Net;
using System.Net.Sockets;

namespace ServiceConvertPDFToJepeg
{
    public class Network
    {
        public static int Port = 8005;
        public IPEndPoint IpPoint;
        public Socket ListenSocket;

        public Network()
        {
            IpPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
            ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ListenSocket.Bind(IpPoint);    
        }
    }
}