using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstStore.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace MyFirstStore.Services
{
    public class BuilderBackColorCSS
    {
        private readonly ILogger<BuilderBackColorCSS> _logger;
        public BuilderBackColorCSS(ILogger<BuilderBackColorCSS> logger)
        {
            _logger = logger;
        }
        const string pathToCSS = "wwwroot/css/backColor/backColorCSS.css";
        public void AddNewCSS(string HEXColor)
        {
            string textCSS;
            textCSS = CreateCSSstyle(HEXColor);
            WriteCSSAsync(textCSS, FileMode.Append);
        }
        public async void RefreshCSS(string oldHEXColor, string newHEXColor)
        {
            string textCSS;
            textCSS = await ReadCSSAsync();
            textCSS = ReplacementColor(textCSS, oldHEXColor, newHEXColor);
            WriteCSSAsync(textCSS, FileMode.Create);
        }
        private async Task<string> ReadCSSAsync()
        {
            string textCSS = "";
            using (StreamReader streamReader = new StreamReader(pathToCSS, System.Text.Encoding.Default))
            {
                textCSS = await streamReader.ReadToEndAsync();
            }
            return textCSS;
        }
        private string CreateCSSstyle(string HEXColor)
        {
            StringBuilder stringBuilder = new StringBuilder(85);
            stringBuilder.Append($".bc{HEXColor} {{background: #{HEXColor};}}\n");
            stringBuilder.Append($".borderProductBox{HEXColor} {{border: 3px solid #{HEXColor};}}\n");
            return stringBuilder.ToString();
        }
        private string ReplacementColor(string textCSS, string oldHEXColor, string newHEXColor)
        {
            return textCSS.Replace(oldHEXColor, newHEXColor);
        }
        private async void WriteCSSAsync(string textCSS, FileMode fileMode)
        {
            using (FileStream fileStream = new FileStream(pathToCSS, fileMode))
            {
                try
                {
                    byte[] array = System.Text.Encoding.Default.GetBytes(textCSS);
                    await fileStream.WriteAsync(array, 0, array.Length);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Fail write css");
                }
            }
        }

    }
} 