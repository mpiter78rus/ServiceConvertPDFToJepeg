using System;
using System.Net;
using Serilog;


namespace ServiceConvertPDFToJepeg
{
    public class Network
    {
        
        public readonly HttpListener Listener;
        
        public Network(string _prefixe)
        {
            if (!HttpListener.IsSupported)
            {
                Log.Logger.Information("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            
            if (_prefixe == null || _prefixe.Length == 0)
                throw new ArgumentException("prefixe");
            
            Listener = new HttpListener();
            Listener.Prefixes.Add(_prefixe);

        }
   
        
    }
}