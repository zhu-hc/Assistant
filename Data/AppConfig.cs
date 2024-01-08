using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandyControl.Themes;

namespace Assistant.Data
{
    internal class AppConfig
    {
        public static readonly string SavePath = $"{AppDomain.CurrentDomain.BaseDirectory}AppConfig.json";

        public string Lang { get; set; } = "zh-cn";

        public ApplicationTheme Theme { get; set; } = ApplicationTheme.Light;
    }
}
