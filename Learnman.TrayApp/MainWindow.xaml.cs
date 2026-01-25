using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Learnman.TrayApp
{
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon = null!;
        private bool _isServerRunning = false;
        private Process? _serverProcess;

        private ThemeManager _themeManager;

        private bool _isUserSelection = true;

        public MainWindow()
        {
            InitializeComponent();
            _themeManager = new ThemeManager(); // Initialize theme sync
            _themeManager.ThemeChanged += OnThemeChanged;
            
            // Set initial state
            OnThemeChanged(_themeManager.CurrentTheme);
            
            InitializeTrayIcon();
            
            // Start centered but show up smoothly
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void OnThemeChanged(string theme)
        {
            Dispatcher.Invoke(() => 
            {
                _isUserSelection = false;
                foreach (ComboBoxItem item in ThemeComboBox.Items)
                {
                    if (item.Tag.ToString() == theme)
                    {
                        ThemeComboBox.SelectedItem = item;
                        break;
                    }
                }
                _isUserSelection = true;
            });
        }

        private void ThemeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_isUserSelection && ThemeComboBox.SelectedItem is ComboBoxItem item)
            {
                string theme = item.Tag.ToString() ?? "light";
                _themeManager.SetTheme(theme);
            }
        }

        private System.Windows.Forms.ToolStripMenuItem _startMenuItem = null!;
        private System.Windows.Forms.ToolStripMenuItem _stopMenuItem = null!;

        private void InitializeTrayIcon()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            try
            {
                // Load icon from file
                string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.png");
                if (System.IO.File.Exists(iconPath))
                {
                    using (var bitmap = new System.Drawing.Bitmap(iconPath))
                    {
                        var handle = bitmap.GetHicon();
                        _notifyIcon.Icon = System.Drawing.Icon.FromHandle(handle);
                    }
                }
                else
                {
                    _notifyIcon.Icon = System.Drawing.SystemIcons.Application;
                }
            }
            catch
            {
                _notifyIcon.Icon = System.Drawing.SystemIcons.Application;
            }
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "LearnMan Tray App";
            
            _notifyIcon.DoubleClick += (s, args) => ShowWindow();
            
            // Context Menu
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Renderer = new System.Windows.Forms.ToolStripProfessionalRenderer(new DarkColorTable());
            contextMenu.BackColor = System.Drawing.Color.FromArgb(26, 26, 46); // Dark Blue #1A1A2E
            contextMenu.ForeColor = System.Drawing.Color.White;
            contextMenu.ShowImageMargin = false; // Clean look
            
            _startMenuItem = new System.Windows.Forms.ToolStripMenuItem("Start Server", null, (s, e) => {
                if (!_isServerRunning) ToggleServerButton_Click(null!, null!);
            });
            
            _stopMenuItem = new System.Windows.Forms.ToolStripMenuItem("Stop Server", null, (s, e) => {
                if (_isServerRunning) ToggleServerButton_Click(null!, null!);
            });
            _stopMenuItem.Enabled = false; // Initially stopped

            var runAppItem = new System.Windows.Forms.ToolStripMenuItem("Run App", null, (s, e) => RunAppButton_Click(null!, null!));

            contextMenu.Items.Add(CreateMenuItem("Open", (s, e) => ShowWindow()));
            contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            contextMenu.Items.Add(_startMenuItem);
            contextMenu.Items.Add(_stopMenuItem);
            contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            contextMenu.Items.Add(runAppItem);
            contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            contextMenu.Items.Add(CreateMenuItem("Exit", (s, e) => ExitApplication()));
            
            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private System.Windows.Forms.ToolStripMenuItem CreateMenuItem(string text, EventHandler onClick)
        {
            return new System.Windows.Forms.ToolStripMenuItem(text, null, onClick);
        }

        // Custom Color Table for Dark Theme
        public class DarkColorTable : System.Windows.Forms.ProfessionalColorTable
        {
            public override System.Drawing.Color MenuItemSelected => System.Drawing.Color.FromArgb(99, 102, 241); // Indigo
            public override System.Drawing.Color MenuItemBorder => System.Drawing.Color.FromArgb(99, 102, 241);
            public override System.Drawing.Color MenuBorder => System.Drawing.Color.FromArgb(60, 60, 60);
            public override System.Drawing.Color ToolStripDropDownBackground => System.Drawing.Color.FromArgb(26, 26, 46);
            public override System.Drawing.Color ImageMarginGradientBegin => System.Drawing.Color.FromArgb(26, 26, 46);
            public override System.Drawing.Color ImageMarginGradientMiddle => System.Drawing.Color.FromArgb(26, 26, 46);
            public override System.Drawing.Color ImageMarginGradientEnd => System.Drawing.Color.FromArgb(26, 26, 46);
            public override System.Drawing.Color SeparatorDark => System.Drawing.Color.FromArgb(60, 60, 60);
            public override System.Drawing.Color SeparatorLight => System.Drawing.Color.Transparent;
            public override System.Drawing.Color MenuItemSelectedGradientBegin => System.Drawing.Color.FromArgb(99, 102, 241);
            public override System.Drawing.Color MenuItemSelectedGradientEnd => System.Drawing.Color.FromArgb(99, 102, 241);
            public override System.Drawing.Color MenuItemPressedGradientBegin => System.Drawing.Color.FromArgb(99, 102, 241);
            public override System.Drawing.Color MenuItemPressedGradientEnd => System.Drawing.Color.FromArgb(99, 102, 241);
        }

        private void ShowWindow()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        private void ExitApplication()
        {
            StopServer();
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide(); // Minimize to tray
        }

        private void ExitApplication_Click(object sender, RoutedEventArgs e)
        {
            ExitApplication();
        }

        private void ToggleServerButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isServerRunning)
            {
                StopServer();
            }
            else
            {
                StartServer();
            }
        }

        private void StartServer()
        {
            try
            {
                if (_serverProcess != null && !_serverProcess.HasExited) return;

                string port = PortTextBox.Text;
                
                string currentDir = AppDomain.CurrentDomain.BaseDirectory;
                string exePath = System.IO.Path.Combine(currentDir, "Learnman.exe");
                string dllPath = System.IO.Path.Combine(currentDir, "Learnman.dll");
                
                ProcessStartInfo psi;
                
                // Bind to all interfaces to avoid localhost ambiguity (IPv4 vs IPv6)
                string urlArg = $"--urls=http://0.0.0.0:{port}";

                if (System.IO.File.Exists(exePath))
                {
                     psi = new ProcessStartInfo
                    {
                        FileName = exePath,
                        Arguments = urlArg,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WorkingDirectory = System.IO.Path.GetDirectoryName(exePath)
                    };
                }
                else if (System.IO.File.Exists(dllPath))
                {
                    psi = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"{dllPath} {urlArg}",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                }
                else
                {
                   var solutionDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDir, @"..\..\..\..")); 
                   var projectPath = System.IO.Path.Combine(solutionDir, "Learnman", "Learnman.csproj");
                   
                   if (System.IO.File.Exists(projectPath))
                   {
                        psi = new ProcessStartInfo
                        {
                            FileName = "dotnet",
                            Arguments = $"run --project \"{projectPath}\" {urlArg}",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WorkingDirectory = System.IO.Path.GetDirectoryName(projectPath),
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };
                   }
                else
                {
                    CustomMessageBox.Show($"Could not find Learnman application to start.\nSearched:\n{exePath}\n{dllPath}", "Server Not Found");
                    return;
                }
                }

                _serverProcess = new Process { StartInfo = psi };
                
                _serverProcess.OutputDataReceived += (s, e) => Debug.WriteLine($"SERVER OUT: {e.Data}");
                _serverProcess.ErrorDataReceived += (s, e) => {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                         Debug.WriteLine($"SERVER ERR: {e.Data}");
                         // Optional: Log to file or show if critical
                    }
                };

                _serverProcess.Start();
                _serverProcess.BeginOutputReadLine();
                _serverProcess.BeginErrorReadLine();

                // Check if it exits immediately
                if (_serverProcess.WaitForExit(2000)) // Wait 2 seconds to see if it dies
                {
                     CustomMessageBox.Show($"Server exited prematureley. Code: {_serverProcess.ExitCode}", "Server Error");
                     _serverProcess = null;
                     _isServerRunning = false;
                }
                else
                {
                    _isServerRunning = true;
                }
                
                UpdateServerState();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Failed to start server: {ex.Message}", "Error");
            }
        }

        private void StopServer()
        {
            try
            {
                if (_serverProcess != null && !_serverProcess.HasExited)
                {
                    // Try to be nice first? No, just kill for now to be sure.
                    _serverProcess.Kill(true); 
                    _serverProcess.WaitForExit(1000);
                }
            }
            catch { /* Ignore errors on kill */ }
            finally
            {
                _serverProcess = null;
                _isServerRunning = false;
                UpdateServerState();
            }
        }

        private void UpdateServerState()
        {
            if (_isServerRunning)
            {
                StatusDot.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(46, 204, 113)); // Green
                StatusText.Text = "RUNNING";
                StatusText.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(46, 204, 113));
                ToggleServerButton.Content = "STOP SERVER";
                ToggleServerButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Red
                
                if (_startMenuItem != null) _startMenuItem.Enabled = false;
                if (_stopMenuItem != null) _stopMenuItem.Enabled = true;
            }
            else
            {
                StatusDot.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60)); // Red
                StatusText.Text = "STOPPED";
                StatusText.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60));
                ToggleServerButton.Content = "START SERVER";
                ToggleServerButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(15, 52, 96)); // Blue
                
                if (_startMenuItem != null) _startMenuItem.Enabled = true;
                if (_stopMenuItem != null) _stopMenuItem.Enabled = false;
            }
        }

        private void RunAppButton_Click(object sender, RoutedEventArgs e)
        {
            string port = PortTextBox.Text;
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"http://localhost:{port}",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Could not open browser: {ex.Message}", "Browser Error");
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
        }
    }
}