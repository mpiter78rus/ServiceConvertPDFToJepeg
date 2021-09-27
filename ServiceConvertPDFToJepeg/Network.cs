using System.Dynamic;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Reflection.Emit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


namespace ServiceConvertPDFToJepeg
{
    public class Network
    {
        
        public IPEndPoint IpPoint;
        public Socket ListenSocket;
        public IConfiguration AppConf { get; set; }
        
        public Network(int _port, string _host)
        {
            IpPoint = new IPEndPoint(IPAddress.Parse(_host), _port);
            ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ListenSocket.Bind(IpPoint);
        }
   
        
    }
}