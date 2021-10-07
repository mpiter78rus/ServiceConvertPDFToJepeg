using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ServiceConvertPDFToJepeg
{
    public class Worker : BackgroundService
    {
        public IConfigurationRoot Configuration { get; }
        private readonly Network _nwk;
        GhostConvert _cvt;
        public Worker(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            string _outputPdfPath = Configuration.GetValue<string>("OUTPUTPDFPATH", "D:\\tmpl\\pdf output\\");
            string _dllPath = Configuration.GetValue<string>("DLLGHOSTPATH", "C:\\Program Files\\gs\\gs9.54.0\\bin\\gsdll64.dl");
            int _dpi = int.Parse(Configuration.GetValue<string>("DPI", "96"));
            string _httpHost = Configuration.GetValue<string>("HTTPHOST", "http://localhost:8888/");
            _nwk = new Network(_httpHost);
            _cvt = new GhostConvert(_outputPdfPath, _dllPath, _dpi);
        }
        
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _nwk.Listener.Start();
            Log.Logger.Information("The service has been launched");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _nwk.Listener.Close();
            Log.Logger.Information("The service is closed");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                HttpListenerContext context = await _nwk.Listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                
                Stream body = request.InputStream;
                Encoding encoding = request.ContentEncoding;
                StreamReader reader = new StreamReader(body, encoding);
                
                Data result = JsonSerializer.Deserialize<Data>(reader.ReadToEnd());

                for (int i = 0; i < result.Paths.Count; i++)
                {
                   _cvt.ConvertPDF(result.Paths[i]); 
                }
                
                byte[] responceData = Encoding.UTF8.GetBytes("OK");
                response.ContentType = "text/html";
                response.ContentEncoding = Encoding.UTF8;
                response.ContentLength64 = responceData.LongLength;
                Stream output = response.OutputStream;
                await output.WriteAsync(responceData, 0, responceData.Length, stoppingToken);
                output.Close();
  
            }
            await ExecuteAsync(stoppingToken);
        }
    }
}