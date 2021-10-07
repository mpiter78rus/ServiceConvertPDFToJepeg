using System;
using System.Drawing.Imaging;
using System.IO;
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;

namespace ServiceConvertPDFToJepeg
{
    public class GhostConvert
    {
        
        int desired_dpi;
        string inputPdfPath;
        string outputPdfPath;
        public GhostscriptRasterizer Rasterizer;
        GhostscriptVersionInfo gvi;
        
        public GhostConvert(string _outputPdfPath, string _dllPath, int _dpi)
        {
            gvi = new GhostscriptVersionInfo(_dllPath);
            outputPdfPath = _outputPdfPath;
            desired_dpi = _dpi;
            Rasterizer = new GhostscriptRasterizer();
        }

        public void ConvertPDF(string _inputPdfPath)
        {
            inputPdfPath = _inputPdfPath;
            Rasterizer.Open(inputPdfPath, gvi, false);
            for (int pageNumber = 1; pageNumber <= Rasterizer.PageCount; pageNumber++)
            {
                var pageFilePath = Path.Combine(outputPdfPath, $"{Guid.NewGuid()}-{pageNumber}.jpeg");
                var img = Rasterizer.GetPage(desired_dpi, pageNumber);
                img.Save(pageFilePath, ImageFormat.Jpeg);
                    
            }
            Rasterizer.Close();
        } 
        
    }
}