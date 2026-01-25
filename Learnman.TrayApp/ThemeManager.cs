using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Learnman.TrayApp
{
    public class ThemeManager
    {
        private FileSystemWatcher? _watcher;
        private string _themeFile;

        public event Action<string>? ThemeChanged;

        public string CurrentTheme { get; private set; } = "light";

        public ThemeManager()
        {
            // Use AppData for shared state between Web App and Tray App
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string learnmanDir = Path.Combine(appData, "Learnman");
            
            if (!Directory.Exists(learnmanDir))
            {
                Directory.CreateDirectory(learnmanDir);
            }

            _themeFile = Path.Combine(learnmanDir, "theme.lock");
            
            InitializeWatcher();
            LoadTheme(); // Initial load
        }

        private void InitializeWatcher()
        {
            try
            {
                string? dir = Path.GetDirectoryName(_themeFile);
                if (dir != null && Directory.Exists(dir))
                {
                    _watcher = new FileSystemWatcher(dir, "theme.lock");
                    _watcher.NotifyFilter = NotifyFilters.LastWrite;
                    _watcher.Changed += OnThemeChanged;
                    _watcher.EnableRaisingEvents = true;
                }
            }
            catch { /* Ignore watcher errors */ }
        }

        private void OnThemeChanged(object sender, FileSystemEventArgs e)
        {
            // Debounce slightly or just run on UI thread
            System.Windows.Application.Current.Dispatcher.Invoke(() => 
            {
                // Small delay to ensure write is complete
                System.Threading.Tasks.Task.Delay(100).ContinueWith(_ => 
                {
                     System.Windows.Application.Current.Dispatcher.Invoke(LoadTheme);
                });
            });
        }

        private void LoadTheme()
        {
            try
            {
                if (File.Exists(_themeFile))
                {
                    string theme = File.ReadAllText(_themeFile).Trim();
                    ApplyTheme(theme);
                }
                else
                {
                    ApplyTheme("light"); // Default to match Web App
                }
            }
            catch { /* Ignore read errors */ }
        }

        private void ApplyTheme(string theme)
        {
            CurrentTheme = theme;
            var dict = System.Windows.Application.Current.Resources;
            
            // Notify listeners (UI)
            ThemeChanged?.Invoke(theme);

            if (theme == "light")
            {
                // Light Theme
                dict["bg-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F8FAFC"));
                dict["bg-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFFFFF"));
                dict["text-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0F172A"));
                dict["text-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#334155"));
                dict["accent-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4F46E5")); // Indigo
                dict["accent-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#DB2777")); // Pink
                dict["accent-danger"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#EF4444")); // Red-500 (Standard)
                dict["accent-success"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#22C55E")); // Green-500
            }
            else if (theme == "sunset")
            {
                // Sunset Theme
                dict["bg-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1A1412"));
                dict["bg-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2A1F1A"));
                dict["text-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FEF3C7"));
                dict["text-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#D6B89F"));
                dict["accent-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FB923C")); // Orange
                dict["accent-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#EF4444")); // Red
                dict["accent-danger"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#BE123C")); // Rose-700 (Warm Burgundy)
                dict["accent-success"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#34D399")); // Emerald-400
            }
            else if (theme == "ocean")
            {
                // Ocean Theme
                dict["bg-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0C1929"));
                dict["bg-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#132337"));
                dict["text-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E0F2FE"));
                dict["text-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#7DD3FC"));
                dict["accent-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0EA5E9")); // Sky
                dict["accent-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#14B8A6")); // Teal
                dict["accent-danger"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F43F5E")); // Rose-500 (Cool Red)
                dict["accent-success"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2DD4BF")); // Teal-400
            }
            else if (theme == "forest")
            {
                // Forest Theme
                dict["bg-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0F1A14"));
                dict["bg-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1A2A1F"));
                dict["text-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#DCFCE7"));
                dict["text-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#86EFAC"));
                dict["accent-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#22C55E")); // Green
                dict["accent-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#84CC16")); // Lime
                dict["accent-danger"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#B91C1C")); // Red-700 (Deep Red)
                dict["accent-success"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#84CC16")); // Lime-500
            }
            else
            {
                // Dark Theme (Default)
                dict["bg-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0F0C1A"));
                dict["bg-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1A162E"));
                dict["text-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFFFFF"));
                dict["text-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#94A3B8"));
                dict["accent-primary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#8B5CF6")); // Violet
                dict["accent-secondary"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#06B6D4")); // Cyan
                dict["accent-danger"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CF6679")); // Material Dark Error (Desaturated Red)
                dict["accent-success"] = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4ADE80")); // Green-400
            }
        }

        public void SetTheme(string theme)
        {
            try
            {
                // Write to file to trigger sync
                File.WriteAllText(_themeFile, theme);
            }
            catch { /* Ignore write errors */ }
        }
    }
}
