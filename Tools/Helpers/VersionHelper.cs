using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Assistant.Tools.Helpers
{
    internal class VersionHelper
    {
        internal static String GetVersion()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location);
            return $"V{versionInfo.FileVersion}";
        }

        internal static String GetCopyright() =>
            FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location).LegalCopyright!;
    }
}
