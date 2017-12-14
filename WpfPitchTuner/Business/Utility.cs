using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WpfPitchTuner.Business
{
    public static class Utility
    {
        public static Version GetProductVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            return new Version(fileVersionInfo.ProductVersion);
        }
    }
}
