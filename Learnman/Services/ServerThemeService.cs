using System;
using System.IO;

namespace Learnman.Services
{
    public class ServerThemeService
    {
        private readonly string _themeFile;

        public ServerThemeService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string learnmanDir = Path.Combine(appData, "Learnman");
            _themeFile = Path.Combine(learnmanDir, "theme.lock");
        }

        public string GetCurrentTheme()
        {
            try
            {
                if (File.Exists(_themeFile))
                {
                    return File.ReadAllText(_themeFile).Trim();
                }
            }
            catch
            {
                // Ignore errors, fallback to light
            }
            return "light";
        }
    }
}
