using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using Microsoft.Extensions.Hosting;
using System.Drawing.Imaging;
using Serilog;

namespace ServiceConvertPDFToJepeg
{
    public class Worker : BackgroundService
    {
        private readonly Network _nwk = new Network();
        int desired_dpi = 96;
        string outputPdfPath = @"D:\tmpl\pdf output\";
        GhostscriptVersionInfo gvi = new GhostscriptVersionInfo(@"C:\Program Files\gs\gs9.54.0\bin\gsdll64.dll");
        
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _nwk.ListenSocket.Listen(10);
            Log.Logger.Information("The service has been launched");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _nwk.ListenSocket.Close();
            Log.Logger.Information("The service is closed");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var handler = await _nwk.ListenSocket.AcceptAsync();
                var builder = new StringBuilder();
                var bytes = 0;
                var data = new byte[256];

                do
                {
                    bytes = handler.Receive(data);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (handler.Available>0);

                Log.Logger.Information(builder.ToString());
                
                /*using (var rasterizer = new GhostscriptRasterizer())
                {
                    rasterizer.Open(builder.ToString(), gvi, false);

                    for (int pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                    {
                        var pageFilePath = Path.Combine(outputPdfPath, $"{Guid.NewGuid()}-{pageNumber}.jpeg");
                        var img = rasterizer.GetPage(desired_dpi, pageNumber);
                        img.Save(pageFilePath, ImageFormat.Jpeg);
                    
                    }
             
                }*/
                
                var message = "message received";
                data = Encoding.Unicode.GetBytes(message);
                handler.Send(data);
                    
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();   
            }
            await ExecuteAsync(stoppingToken);
        }
    }
}