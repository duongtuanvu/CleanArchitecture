using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Application.Extensions
{
    public class PdfExtension
    {
        public IConverter converter { get; set; }
        public byte[] ExportPdf<T>(IEnumerable<T> data, bool isSaveFile = false, string pathSaveFile = null) where T : class
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report"
            };
            if (isSaveFile)
            {
                globalSettings.Out = pathSaveFile;
            }
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = "",
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            return converter.Convert(pdf);
            //var file = converter.Convert(pdf);
        }
    }
}
