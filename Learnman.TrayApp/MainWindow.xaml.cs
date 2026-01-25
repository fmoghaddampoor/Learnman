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
                
                // Update Tray Menu Colors
                UpdateTrayTheme(theme);
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

        private System.Windows.Forms.ContextMenuStrip _contextMenu = null!;

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
            _contextMenu = new System.Windows.Forms.ContextMenuStrip();
            _contextMenu.ShowImageMargin = false; // Clean look
            
            _startMenuItem = new System.Windows.Forms.ToolStripMenuItem("Start Server", null, (s, e) => {
                if (!_isServerRunning) ToggleServerButton_Click(null!, null!);
            });
            
            _stopMenuItem = new System.Windows.Forms.ToolStripMenuItem("Stop Server", null, (s, e) => {
                if (_isServerRunning) ToggleServerButton_Click(null!, null!);
            });
            _stopMenuItem.Enabled = false; // Initially stopped

            var runAppItem = new System.Windows.Forms.ToolStripMenuItem("Run App", null, (s, e) => RunAppButton_Click(null!, null!));

            _contextMenu.Items.Add(CreateMenuItem("Open", (s, e) => ShowWindow()));
            _contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            _contextMenu.Items.Add(_startMenuItem);
            _contextMenu.Items.Add(_stopMenuItem);
            _contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            _contextMenu.Items.Add(runAppItem);
            _contextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            _contextMenu.Items.Add(CreateMenuItem("Exit", (s, e) => ExitApplication()));
            
            _notifyIcon.ContextMenuStrip = _contextMenu;
        }

        private void UpdateTrayTheme(string theme)
        {
            if (_contextMenu == null) return;

            System.Drawing.Color bgColor, textColor, accentColor;

            switch (theme.ToLower())
            {
                case "light":
                    bgColor = System.Drawing.Color.FromArgb(248, 250, 252); // #F8FAFC
                    textColor = System.Drawing.Color.FromArgb(15, 23, 42);   // #0F172A
                    accentColor = System.Drawing.Color.FromArgb(79, 70, 229); // #4F46E5 (Indigo)
                    break;
                case "sunset":
                    bgColor = System.Drawing.Color.FromArgb(26, 20, 18);     // #1A1412
                    textColor = System.Drawing.Color.FromArgb(254, 243, 199); // #FEF3C7
                    accentColor = System.Drawing.Color.FromArgb(251, 146, 60); // #FB923C (Orange)
                    break;
                case "ocean":
                    bgColor = System.Drawing.Color.FromArgb(12, 25, 41);      // #0C1929
                    textColor = System.Drawing.Color.FromArgb(224, 242, 254); // #E0F2FE
                    accentColor = System.Drawing.Color.FromArgb(14, 165, 233); // #0EA5E9 (Sky)
                    break;
                case "forest":
                    bgColor = System.Drawing.Color.FromArgb(15, 26, 20);      // #0F1A14
                    textColor = System.Drawing.Color.FromArgb(220, 252, 231); // #DCFCE7
                    accentColor = System.Drawing.Color.FromArgb(34, 197, 94);   // #22C55E (Green)
                    break;
                default: // Dark
                    bgColor = System.Drawing.Color.FromArgb(26, 26, 46);      // #1A1A2E (Matches existing dark)
                    textColor = System.Drawing.Color.White;
                    accentColor = System.Drawing.Color.FromArgb(139, 92, 246);  // #8B5CF6 (Violet)
                    break;
            }

            _contextMenu.BackColor = bgColor;
            _contextMenu.ForeColor = textColor;
            _contextMenu.Renderer = new System.Windows.Forms.ToolStripProfessionalRenderer(new ThemeColorTable(bgColor, textColor, accentColor));
        }

        private System.Windows.Forms.ToolStripMenuItem CreateMenuItem(string text, EventHandler onClick)
        {
            return new System.Windows.Forms.ToolStripMenuItem(text, null, onClick);
        }

        // Custom Color Table for Dynamic Theme
        public class ThemeColorTable : System.Windows.Forms.ProfessionalColorTable
        {
            private readonly System.Drawing.Color _bgColor;
            private readonly System.Drawing.Color _textColor;
            private readonly System.Drawing.Color _accentColor;

            public ThemeColorTable(System.Drawing.Color bg, System.Drawing.Color text, System.Drawing.Color accent)
            {
                _bgColor = bg;
                _textColor = text;
                _accentColor = accent;
            }

            public override System.Drawing.Color MenuItemSelected => _accentColor;
            public override System.Drawing.Color MenuItemBorder => _accentColor;
            public override System.Drawing.Color MenuBorder => _bgColor;
            public override System.Drawing.Color ToolStripDropDownBackground => _bgColor;
            public override System.Drawing.Color ImageMarginGradientBegin => _bgColor;
            public override System.Drawing.Color ImageMarginGradientMiddle => _bgColor;
            public override System.Drawing.Color ImageMarginGradientEnd => _bgColor;
            public override System.Drawing.Color SeparatorDark => _textColor; // Use text color for separators
            public override System.Drawing.Color SeparatorLight => System.Drawing.Color.Transparent;
            public override System.Drawing.Color MenuItemSelectedGradientBegin => _accentColor;
            public override System.Drawing.Color MenuItemSelectedGradientEnd => _accentColor;
            public override System.Drawing.Color MenuItemPressedGradientBegin => _accentColor;
            public override System.Drawing.Color MenuItemPressedGradientEnd => _accentColor;
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
                // RUNNING STATE
                StatusDot.SetResourceReference(System.Windows.Shapes.Shape.FillProperty, "accent-success");
                StatusText.Text = "RUNNING";
                StatusText.SetResourceReference(TextBlock.ForegroundProperty, "accent-success");
                
                ToggleServerButton.Content = "STOP SERVER";
                ToggleServerButton.SetResourceReference(System.Windows.Controls.Control.BackgroundProperty, "accent-danger");
                
                if (_startMenuItem != null) _startMenuItem.Enabled = false;
                if (_stopMenuItem != null) _stopMenuItem.Enabled = true;
            }
            else
            {
                // STOPPED STATE
                StatusDot.SetResourceReference(System.Windows.Shapes.Shape.FillProperty, "accent-danger");
                StatusText.Text = "STOPPED";
                StatusText.SetResourceReference(TextBlock.ForegroundProperty, "accent-danger");
                
                ToggleServerButton.Content = "START SERVER";
                // Restore dynamic binding to accent-primary
                ToggleServerButton.SetResourceReference(System.Windows.Controls.Control.BackgroundProperty, "accent-primary");
                
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