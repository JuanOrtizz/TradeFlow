using System;
using System.IO;

namespace TradeFlow.Helpers
{
    // Todo se guarda en una sola carpeta por PC: %LOCALAPPDATA%\TradeFlow en Windows.
    public static class AppPaths
    {
        public static string DirectorioDatos => OperatingSystem.IsWindows()
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TradeFlow")
            : FileSystem.AppDataDirectory;

        public static string ObtenerRuta(string nombreArchivo)
            => Path.Combine(DirectorioDatos, nombreArchivo);
    }
}