using System;
using System.Runtime.InteropServices;

namespace AudiosAmigo.Windows
{
    internal static class IconExtract
    {
        [DllImport("Shell32.dll")]
        public static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);
    }
}
